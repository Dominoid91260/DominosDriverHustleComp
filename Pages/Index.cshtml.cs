using DominosDriverHustleComp.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DominosDriverHustleComp.Pages
{
    public class IndexModel : PageModel
    {
        public class DriverViewModel
        {
            // Used for "Average" result
            public bool IsSpecialResult = false;

            public required string Name { get; set; }
            public int NumDels { get; set; }
            public TimeSpan AvgHustleOut { get; set; }
            public TimeSpan AvgHustleIn { get; set; }
            public TimeSpan AvgHustleCombined { get => AvgHustleIn + AvgHustleOut; }
        }

        private readonly HustleCompContext _context;
        public IEnumerable<DriverViewModel>? Drivers;

        public IndexModel(HustleCompContext context)
        {
            _context = context;
        }

        public void OnGet(DateTime? WeekEnding)
        {
            if (WeekEnding.HasValue)
            {
                var wkEnding = WeekEnding.Value;

                Drivers = _context.Deliveries
                    .Include(d => d.Driver)
                    .AsEnumerable()
                    .Where(d => d.DispatchedAt <= wkEnding && (wkEnding - d.DispatchedAt).TotalDays <= 7)
                    .GroupBy(d => d.Driver)
                    .Select(g => new DriverViewModel()
                    {
                        Name = g.Key.FirstName + " " + g.Key.LastName,
                        NumDels = g.Count(),
                        AvgHustleOut = TimeSpan.FromSeconds(Math.Floor(g.Select(d => (d.LeftStoreAt - d.DispatchedAt).TotalSeconds).Average())),
                        AvgHustleIn = TimeSpan.FromSeconds(Math.Floor(g.Select(d => (d.InAt - d.StoreEnteredAt).TotalSeconds).Average()))
                    });
            }
        }
    }
}
