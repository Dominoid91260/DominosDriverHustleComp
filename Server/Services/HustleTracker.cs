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
        public StoreLocation StoreLocation { get; set; }
        private Dictionary<int, DriverHustleData> DriverHustle = [];

        public void StoreHustleData(DriverUpdate update)
        {
            if (update is null ||
                update.HeightenedTimeAwareness is null ||
                update.HeightenedTimeAwareness.DispatchAt is null ||
                update.HeightenedTimeAwareness.LeftStoreAt is null ||
                update.HeightenedTimeAwareness.StoreEnterAt is null ||
                update.HeightenedTimeAwareness.InAt is null)
                return;

            var hta = update.HeightenedTimeAwareness;
            var data = new HustleData
            {
                HustleOut = (float)(hta.LeftStoreAt - hta.DispatchAt).Value.TotalSeconds,
                HustleIn = (float)(hta.InAt - hta.StoreEnterAt).Value.TotalSeconds
            };

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
                    HustleData = [ data ]
                });
            }
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
