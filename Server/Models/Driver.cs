﻿namespace DominosDriverHustleComp.Server.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsPermanentlyDisqualified { get; set; }

        public virtual IEnumerable<Delivery> Deliveries { get; set; }
        public virtual IEnumerable<DeliverySummary> DeliverySummaries { get; set; }
    }
}
