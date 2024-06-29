using DominosDriverHustleComp.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthToken : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public class AuthModel
        {
            public required string BearerToken { get; set; }
        }

        public AuthToken(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public void Post([FromBody] AuthModel model)
        {
            var scope = _serviceProvider.CreateScope();
            var sseService = scope.ServiceProvider.GetRequiredService<GPSSSEService>();
            sseService.BearerToken = model.BearerToken;

            var gpsService = scope.ServiceProvider.GetRequiredService<GPSDashboardService>();
            gpsService.StopBrowser();
        }
    }
}
