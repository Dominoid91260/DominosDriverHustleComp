using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DominosDriverHustleComp.Server.Services
{
    public class ReportGeneratorService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportGeneratorService> _logger;

        private DateTime _weekEnding;

        private class GroupedModel
        {
            public Driver Driver { get; set; }
            public IEnumerable<Delivery> Deliveries { get; set; }
        }

        public ReportGeneratorService(IServiceProvider serviceProvider, ILogger<ReportGeneratorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private void GenerateDeliverySummaries()
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

            if (!context.Deliveries.Any())
            {
                _logger.LogWarning("No deliveries, skip generating delivery summaries...");
                return;
            }

            var grouped = context.Deliveries.Include(d => d.Driver).GroupBy(d => d.Driver).Select(g => new GroupedModel
            {
                Driver = g.Key,
                Deliveries = g.ToList()
            });

            foreach (var group in grouped)
            {
                var avgOut = group.Deliveries.Average(d => d.AvgHustleOut);
                var avgIn = group.Deliveries.Average(d => d.AvgHustleIn);
                var combined = avgOut + avgIn;

                context.DeliverySummaries.Add(new DeliverySummary
                {
                    Driver = group.Driver,
                    WeekEnding = _weekEnding,
                    AvgHustleOut = avgOut,
                    AvgHustleIn = avgIn,
                    AvgHustleCombined = combined
                });
            }

            // The deliveries table is designed to only store deliveries for a single week.
            // Once the delivery summaries are created we no longer need to keep the deliveries
            // so truncate the table.
            context.Deliveries.ExecuteDelete();
            context.SaveChanges();
        }

        private void GenerateWeeklySummary()
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

            var deliverySummaries = context.DeliverySummaries.Where(ds => ds.WeekEnding  == _weekEnding);

            if (deliverySummaries is null || !deliverySummaries.Any())
            {
                _logger.LogWarning("No delivery summaries for {weekEnding}, skip generating weekly summary.", _weekEnding.ToString("d"));
                return;
            }

            // This should rarely ever happen but will crash the server if it does.
            // This mostly covers the situation where the server is restarted on the same day
            // that reports are generated (Monday)
            var existing = context.WeeklySummaries.Where(ws => ws.WeekEnding == _weekEnding);
            if (existing is not null && existing.Any())
            {
                _logger.LogError("Weekly summary already exists for {weekEnding}, skipping", _weekEnding.ToString("d"));
                return;
            }

            var avgOut = deliverySummaries.Average(ds => ds.AvgHustleOut);
            var avgIn = deliverySummaries.Average(ds => ds.AvgHustleIn);
            var avgCombined = deliverySummaries.Average(ds => ds.AvgHustleCombined);

            context.WeeklySummaries.Add(new WeeklySummary
            {
                WeekEnding = _weekEnding,
                AvgHustleOut = avgOut,
                AvgHustleIn = avgIn,
                AvgHustleCombined = avgCombined
            });

            context.SaveChanges();
        }

        public void GenerateReports()
        {
            var now = DateTime.Now;
            var lastSunday = now.AddDays(-(int)now.DayOfWeek);
            _weekEnding = lastSunday.Date;

            GenerateDeliverySummaries();
            GenerateWeeklySummary();
        }
    }
}
