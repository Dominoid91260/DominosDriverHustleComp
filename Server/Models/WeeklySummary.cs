using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Server.Models
{
    public class WeeklySummary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public DateTime WeekEnding { get; set; }

        public float AvgHustleOut { get; set; }
        public float AvgHustleIn { get; set; }
        public float AvgHustleCombined { get; set; }
    }
}
