using DominosDriverHustleComp.Server.Services;
using DominosDriverHustleComp.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HustleLeaderboard : ControllerBase
    {
        private readonly HustleTracker _hustleTracker;

        public HustleLeaderboard(HustleTracker hustleTracker)
        {
            _hustleTracker = hustleTracker;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(_hustleTracker.GetLeaderboardData());
        }
    }
}
