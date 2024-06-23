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
    }
}
