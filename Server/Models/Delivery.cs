using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Server.Models
{
    public class Delivery
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Driver Driver { get; set; }
        public float AvgHustleOut { get; set; }
        public float AvgHustleIn { get; set; }
        public bool WasTracked { get; set; }
    }
}
