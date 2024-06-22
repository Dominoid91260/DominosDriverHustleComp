using DominosDriverHustleComp.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private readonly ScreenshotService _screenshotService;

        public ScreenshotController(ScreenshotService screenshotService)
        {
            _screenshotService = screenshotService;
        }

        [HttpPost]
        public async Task Post(DateTime WeekEnding)
        {
            await _screenshotService.ScreenshotReport(WeekEnding, CancellationToken.None);
        }
    }
}
