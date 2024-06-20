using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Server.Models
{
    public class WeeklySummary
    {
        [Key]
        public DateTime WeekEnding { get; set; }

        public float AvgHustleOut { get; set; }
        public float AvgHustleIn { get; set; }
        public float AvgHustleCombined { get; set; }

        [ForeignKey("WeekEnding")]
        public IEnumerable<DeliverySummary> DeliverySummaries { get; set; }
    }
}
