using System.ComponentModel.DataAnnotations;

namespace DominosDriverHustleComp.Models
{
    public class Driver
    {
        [Key]
        public required int Id { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required ICollection<DeliveryRecord> Deliveries { get; set; }
    }
}
