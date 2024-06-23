using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly HustleCompContext _context;

        public DriversController(HustleCompContext context)
        {
            _context = context;
        }

        [HttpGet("Disqualifications")]
        public async Task<IEnumerable<DriverDisqualification>> GetDisqualifications()
        {
            return _context.Drivers.Select(d => new DriverDisqualification
            {
                DriverId = d.Id,
                Name = d.FirstName + " " + d.LastName,
                IsDisqualified = d.IsPermanentlyDisqualified
            });
        }

        [HttpPost("Disqualifications/{driverId}")]
        public async Task<IActionResult> PostDisqualifications(int driverId, [FromBody] bool isDisqualified)
        {
            var driver = _context.Drivers.Find(driverId);

            if (driver is null)
                return NotFound();

            driver.IsPermanentlyDisqualified = isDisqualified;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
