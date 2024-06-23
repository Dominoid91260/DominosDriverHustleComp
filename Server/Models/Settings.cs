using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DominosDriverHustleComp.Server.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public float HustleBenchmarkSeconds { get; set; }
        public float OutlierSeconds { get; set; }
        public int MinDels { get; set; }

        [Comment("0-100")]
        [Range(0, 100)]
        public int MinTrackedPercentage { get; set; }

        public int MaxOverspeeds { get; set; }
    }
}
