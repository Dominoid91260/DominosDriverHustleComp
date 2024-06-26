﻿using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Models;
using DominosDriverHustleComp.Server.Services;
using DominosDriverHustleComp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private struct Streaks
        {
            public int WinStreak { get; set; }
            public int OutlierStreak { get; set; }

            public Streaks()
            {
                WinStreak = 0;
                OutlierStreak = 0;
            }
        }

        public class OverspeedModel
        {
            public required Dictionary<int, int> DriverOverspeeds { get; set; }
        }

        private readonly HustleCompContext _context;
        private readonly ScreenshotService _screenshotService;
        private readonly ILogger<ReportsController> _logger;
        private readonly OverspeedsService _overspeedsService;

        public ReportsController(HustleCompContext context, ScreenshotService screenshotService, ILogger<ReportsController> logger, OverspeedsService overspeedsService)
        {
            _context = context;
            _screenshotService = screenshotService;
            _logger = logger;
            _overspeedsService = overspeedsService;
        }

        [HttpGet]
        public async Task<IEnumerable<DateTime>> Get()
        {
            return _context.WeeklySummaries.Select(ws => ws.WeekEnding.Date);
        }

        [HttpGet("{weekEnding}")]
        public async Task<IEnumerable<ReportRecord>> GetSingle(DateTime weekEnding)
        {
            var weeklySummary = await _context.WeeklySummaries
                .Include(ws => ws.DeliverySummaries)
                .ThenInclude(ds => ds.Driver)
                .ThenInclude(d => d.DeliverySummaries)
                .FirstOrDefaultAsync(ws => ws.WeekEnding == weekEnding.Date);

            if (weeklySummary is null)
                return [];

            var records = new List<ReportRecord>
            {
                // create average record
                new() {
                    IsAverageRecord = true,
                    Name = "Average",
                    AvgOut = weeklySummary.AvgHustleOut,
                    AvgIn = weeklySummary.AvgHustleIn
                }
            };

            // add driver records
            records.AddRange(weeklySummary.DeliverySummaries.Select(ds =>
            {
                var streaks = GetStreaksForDriver(ds.Driver, ds.WeekEnding);

                return new ReportRecord
                {
                    Name = ds.Driver.FirstName + " " + ds.Driver.LastName,
                    AvgOut = ds.AvgHustleOut,
                    AvgIn = ds.AvgHustleIn,
                    NumDels = ds.NumDels,
                    TrackedPercentage = ds.TrackedPercentage,
                    PreviousWeekStats = GetPreviousStatsForDriver(ds.Driver, ds.WeekEnding),
                    WinStreak = streaks.WinStreak,
                    Outlier = streaks.OutlierStreak,
                    IsDriverDisqualified = ds.Driver.IsPermanentlyDisqualified,
                    NumOverspeeds = ds.NumOverspeeds
                };
            }));

            return records;
        }

        private static PreviousWeekStats? GetPreviousStatsForDriver(Driver driver, DateTime from)
        {
            if (!driver.DeliverySummaries.Any())
                return null;

            var prevSummaries = driver.DeliverySummaries.Where(ds => ds.WeekEnding < from.Date);

            if (!prevSummaries.Any())
                return null;

            var lastWeek = prevSummaries.OrderByDescending(ds => ds.WeekEnding).First();

            return new PreviousWeekStats
            {
                AvgIn = lastWeek.AvgHustleIn,
                AvgOut = lastWeek.AvgHustleOut
            };
        }

        private Streaks GetStreaksForDriver(Driver driver, DateTime from)
        {
            if (!driver.DeliverySummaries.Any())
                return default;

            var fromDate = from.Date;

            var currentDelSummary = driver.DeliverySummaries.First(ds => ds.WeekEnding == fromDate);
            var currentWeeklySummary = _context.WeeklySummaries.Find(fromDate);

            var settings = _context.Settings.First();

            // Include the requested week in the search, this way a 1 week win will be 1 streak.
            // We can handle displaying this on the client by checking if the streak is 1
            var delSummaries = driver.DeliverySummaries.Where(ds => ds.WeekEnding <= fromDate);

            if (!delSummaries.Any())
                return default;

            bool bCountingWins = currentDelSummary.AvgHustleCombined < settings.HustleBenchmarkSeconds;
            int wins = 0;

            bool bCountingOutliers = currentDelSummary.AvgHustleCombined >= currentWeeklySummary.AvgHustleCombined + settings.OutlierSeconds;
            int outliers = 0;

            foreach (var ds in delSummaries.OrderByDescending(ds => ds.WeekEnding))
            {
                var weeklySummary = _context.WeeklySummaries.Find(ds.WeekEnding);

                if (ds.AvgHustleCombined >= settings.HustleBenchmarkSeconds)
                {
                    bCountingWins = false;
                }
                else if (bCountingWins)
                {
                    wins++;
                }

                if (ds.AvgHustleCombined < weeklySummary.AvgHustleCombined + settings.OutlierSeconds)
                {
                    bCountingOutliers = false;
                }
                else if (bCountingOutliers)
                {
                    outliers++;
                }

                if (!bCountingWins && !bCountingOutliers)
                    break;
            }

            return new Streaks
            {
                WinStreak = wins,
                OutlierStreak = outliers,
            };
        }

        [HttpPost("Overspeeds/{weekEnding}")]
        public async Task<IActionResult> PostOverspeeds(DateTime weekEnding, [FromBody] OverspeedModel overspeeds)
        {
            var weeklySummary = _context.WeeklySummaries.Find(weekEnding.Date);

            if (weeklySummary is null)
                return NotFound();

            _context.Entry(weeklySummary)
                .Collection(ws => ws.DeliverySummaries)
                .Query()
                .Include(ds => ds.Driver)
                .Load();

            foreach (var delSummary in weeklySummary.DeliverySummaries)
            {
                if (overspeeds.DriverOverspeeds.TryGetValue(delSummary.Driver.Id, out var numOverspeeds))
                {
                    delSummary.NumOverspeeds = numOverspeeds;
                }
            }

            await _context.SaveChangesAsync();

            var now = DateTime.Now;
            var lastSunday = now.AddDays(-(int)now.DayOfWeek).Date;

            if (weekEnding.Date == lastSunday)
            {
                _logger.LogInformation("Received overspeeds for {sunday}, screenshotting report", lastSunday.ToString("d"));
                await _screenshotService.ScreenshotReport(lastSunday, CancellationToken.None);
            }

            _overspeedsService.StopBrowser();

            return Ok();
        }
    }
}
