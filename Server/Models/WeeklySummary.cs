using System.ComponentModel.DataAnnotations;

namespace DominosDriverHustleComp.Server.Models
{
    public class WeeklySummary
    {
        [Key]
        public DateTime WeekEnding { get; set; }

        public float AvgHustleOut { get; set; }
        public float AvgHustleIn { get; set; }
        public float AvgHustleCombined { get; set; }
    }
}
