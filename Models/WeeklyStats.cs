using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Models
{
    public class WeeklyStats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public DateTime WeekEnding;

        public required Driver Driver;
        public float AvgHustleOut;
        public float AvgHustleIn;
    }
}
