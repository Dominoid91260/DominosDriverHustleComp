using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Models;
using DominosDriverHustleComp.Server.Models.GPS;
using DominosDriverHustleComp.Shared.ViewModels;

namespace DominosDriverHustleComp.Server.Services
{
    public struct HustleData
    {
        public float HustleOut { get; set; }
        public float HustleIn { get; set; }
    }

    public class DriverHustleData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<HustleData> HustleData { get; set; }
    }

    public class HustleTracker
    {
        private readonly IServiceProvider _serviceProvider;

        public StoreLocation StoreLocation { get; set; }
        private Dictionary<int, DriverHustleData> DriverHustle = [];

        public HustleTracker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void StoreHustleData(DriverUpdate update)
        {
            if (update is null || update.HeightenedTimeAwareness is null)
                return;

            var hta = update.HeightenedTimeAwareness;

            // When a driver is signed into pulse we get an `In` state with a valid HTA, but all the properties are null
            // Hopefully this is a good enough filter to let through valid untracked deliveries
            if (hta.DispatchAt is null &&
                hta.LeftStoreAt is null &&
                hta.StoreEnterAt is null &&
                hta.InAt is null)
                return;

            bool trackDel = true;
            HustleData data = default;

            // One of the HTAs didnt track so mark the entire delivery as untracked
            if (update.HeightenedTimeAwareness.DispatchAt is null ||
                update.HeightenedTimeAwareness.LeftStoreAt is null ||
                update.HeightenedTimeAwareness.StoreEnterAt is null ||
                update.HeightenedTimeAwareness.InAt is null)
            {
                trackDel = false;
            }

            if (trackDel)
            {
                data = new HustleData
                {
                    HustleOut = (float)(hta.LeftStoreAt - hta.DispatchAt).Value.TotalSeconds,
                    HustleIn = (float)(hta.InAt - hta.StoreEnterAt).Value.TotalSeconds
                };

                // The leaderboard is basic and doesnt care about tracked percentage
                // so only store tracked deliveries.
                if (DriverHustle.TryGetValue(update.DriverId, out DriverHustleData? value))
                {
                    value.HustleData.Add(data);
                }
                else
                {
                    DriverHustle.Add(update.DriverId, new DriverHustleData
                    {
                        FirstName = update.FirstName,
                        LastName = update.LastName,
                        HustleData = [data]
                    });
                }
            }

            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

            var driver = context.Drivers.Find(update.DriverId);

            if (driver is null)
            {
                var entry = context.Drivers.Add(new Driver
                {
                    Id = update.DriverId,
                    FirstName = update.FirstName,
                    LastName = update.LastName
                });

                driver = entry.Entity;
            }

            context.Deliveries.Add(new Delivery
            {
                Driver = driver,
                AvgHustleOut = data.HustleOut,
                AvgHustleIn = data.HustleIn,
                WasTracked = trackDel
            });

            context.SaveChanges();
        }

        public IEnumerable<LeaderboardHustle> GetLeaderboardData()
        {
            foreach (var data in  DriverHustle.Values)
            {
                yield return new LeaderboardHustle
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    AvgHustleIn = data.HustleData.Average(d => d.HustleIn),
                    AvgHustleOut = data.HustleData.Average(d => d.HustleOut)
                };
            }
        }
    }
}
