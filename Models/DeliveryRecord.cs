using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosDriverHustleComp.Models
{
    public class DeliveryRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required Driver Driver { get; set; }

        public DateTime DispatchedAt { get; set; }
        public DateTime? LeftStoreAt { get; set; }
        public DateTime? StoreEnteredAt { get; set; }
        public DateTime InAt { get; set; }
    }
}
