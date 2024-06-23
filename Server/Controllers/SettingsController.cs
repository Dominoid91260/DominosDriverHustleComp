using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly HustleCompContext _context;

        public SettingsController(HustleCompContext context)
        {
            _context = context;
        }

        [HttpGet]
        public SettingsVM Get()
        {
            var settings = _context.Settings.First();
            return new SettingsVM
            {
                HustleBenchmarkSeconds = settings.HustleBenchmarkSeconds,
                OutlierSeconds = settings.OutlierSeconds,
                MinDels = settings.MinDels,
                MinTrackedPercentage = settings.MinTrackedPercentage,
                MaxOverspeeds = settings.MaxOverspeeds
            };
        }

        [HttpPost("HustleBenchmarkSeconds")]
        public async Task PostHustleBenchmarkSeconds([FromBody] int hustleBenchmarkSeconds)
        {
            _context.Settings.First().HustleBenchmarkSeconds = hustleBenchmarkSeconds;
            await _context.SaveChangesAsync();
        }

        [HttpPost("OutlierSeconds")]
        public async Task PostOutlierSeconds([FromBody] int outlierSeconds)
        {
            _context.Settings.First().OutlierSeconds = outlierSeconds;
            await _context.SaveChangesAsync();
        }

        [HttpPost("MinDels")]
        public async Task PostMinDels([FromBody] int minDels)
        {
            _context.Settings.First().MinDels = minDels;
            await _context.SaveChangesAsync();
        }

        [HttpPost("MinTrackedPercentage")]
        public async Task PostMinTrackedPercentage([FromBody] int minTrackedPercentage)
        {
            if (minTrackedPercentage < 0 || minTrackedPercentage > 100)
                return; ///@TODO maybe return 404?

            _context.Settings.First().MinTrackedPercentage = minTrackedPercentage;
            await _context.SaveChangesAsync();
        }

        [HttpPost("MaxOverspeeds")]
        public async Task PostMaxOverspeeds([FromBody] int maxOverspeeds)
        {
            _context.Settings.First().MaxOverspeeds = maxOverspeeds;
            await _context.SaveChangesAsync();
        }
    }
}
