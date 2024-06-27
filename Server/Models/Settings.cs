using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DominosDriverHustleComp.Server.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public int HustleBenchmarkSeconds { get; set; }
        public int OutlierSeconds { get; set; }
        public int MinDels { get; set; }

        [Comment("0-100")]
        [Range(0, 100)]
        public int MinTrackedPercentage { get; set; }

        public int MaxOverspeeds { get; set; }
        public bool ShowDeliveries { get; set; }
    }
}
