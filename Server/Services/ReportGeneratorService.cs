﻿using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Models;
using Microsoft.EntityFrameworkCore;

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
                    AvgHustleCombined = combined,
                    NumDels = grouped.Count()
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

            var deliveries = context.Deliveries.AsEnumerable();

            if (deliveries is null || !deliveries.Any())
            {
                _logger.LogWarning("No deliveries, skip generating weekly summary.");
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

            var avgOut = deliveries.Average(d => d.AvgHustleOut);
            var avgIn = deliveries.Average(d => d.AvgHustleIn);
            var avgCombined = avgOut + avgIn;

            context.WeeklySummaries.Add(new WeeklySummary
            {
                WeekEnding = _weekEnding,
                AvgHustleOut = avgOut,
                AvgHustleIn = avgIn,
                AvgHustleCombined = avgCombined
            });

            context.SaveChanges();
        }

        public void GenerateSummaries()
        {
            var now = DateTime.Now;
            var lastSunday = now.AddDays(-(int)now.DayOfWeek);
            _weekEnding = lastSunday.Date;

            // Generate the weekly summary before we save changes otherwise
            // there will be no week-ending to match our foreign key to
            // and the delivery summaries will fail to insert.
            GenerateWeeklySummary();

            GenerateDeliverySummaries();
        }
    }
}
