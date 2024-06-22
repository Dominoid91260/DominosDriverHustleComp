using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Server.Models
{
    public class Delivery
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Driver Driver { get; set; }
        public float HustleOut { get; set; }
        public float HustleIn { get; set; }
        public bool WasTracked { get; set; }
    }
}
