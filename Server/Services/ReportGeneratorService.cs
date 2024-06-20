using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DominosDriverHustleComp.Server.Services
{
    public class ReportGeneratorService
    {
        private readonly IServiceProvider _serviceProvider;

        private DateTime _weekEnding;

        private class GroupedModel
        {
            public Driver Driver { get; set; }
            public IEnumerable<Delivery> Deliveries { get; set; }
        }

        public ReportGeneratorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void GenerateDeliverySummaries()
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

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

            context.SaveChanges();
        }

        private void GenerateWeeklySummary()
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

            var deliverySummaries = context.DeliverySummaries.Where(ds => ds.WeekEnding  == _weekEnding);
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
