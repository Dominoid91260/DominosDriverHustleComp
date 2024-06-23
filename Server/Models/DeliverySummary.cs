using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Server.Models
{
    [Index(nameof(WeekEnding))]
    public class DeliverySummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Driver Driver { get; set; }
        public DateTime WeekEnding { get; set; }
        public float AvgHustleOut { get; set; }
        public float AvgHustleIn { get; set; }
        public float AvgHustleCombined { get; set; }
        public int NumDels { get; set; }

        [Comment("0-100")]
        public int TrackedPercentage { get; set; }

        public int NumOverspeeds { get; set; }
    }
}
