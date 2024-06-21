using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Models;
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

        private readonly HustleCompContext _context;

        public ReportsController(HustleCompContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<DateTime>> Get()
        {
            return _context.WeeklySummaries.Select(ws => ws.WeekEnding);
        }

        [HttpGet("{weekEnding}")]
        public async Task<IEnumerable<ReportRecord>> GetSingle(DateTime weekEnding)
        {
            var weeklySummary = await _context.WeeklySummaries
                .Include(ws => ws.DeliverySummaries)
                .ThenInclude(ds => ds.Driver)
                .ThenInclude(d => d.DeliverySummaries)
                .FirstOrDefaultAsync(ws => ws.WeekEnding == weekEnding);

            if (weeklySummary is null)
                return [];

            return weeklySummary.DeliverySummaries.Select(ds =>
            {
                var streaks = GetStreaksForDriver(ds.Driver, ds.WeekEnding);

                return new ReportRecord
                {
                    Name = ds.Driver.FirstName + " " + ds.Driver.LastName,
                    AvgOut = ds.AvgHustleOut,
                    AvgIn = ds.AvgHustleIn,
                    NumDels = ds.NumDels,
                    PreviousWeekStats = GetPreviousStatsForDriver(ds.Driver, ds.WeekEnding),
                    WinStreak = streaks.WinStreak,
                    Outlier = streaks.OutlierStreak
                };
            });
        }

        private static PreviousWeekStats? GetPreviousStatsForDriver(Driver driver, DateTime from)
        {
            if (!driver.DeliverySummaries.Any())
                return null;

            var prevSummaries = driver.DeliverySummaries.Where(ds => ds.WeekEnding < from);

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

            var currentDelSummary = driver.DeliverySummaries.First(ds => ds.WeekEnding == from);
            var currentWeeklySummary = _context.WeeklySummaries.Find(from);

            var settings = _context.Settings.First();

            // Include the requested week in the search, this way a 1 week win will be 1 streak.
            // We can handle displaying this on the client by checking if the streak is 1
            var delSummaries = driver.DeliverySummaries.Where(ds => ds.WeekEnding <= from);

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
    }
}
