using DominosDriverHustleComp.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace DominosDriverHustleComp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly HustleCompContext _context;

        public ReportsController(HustleCompContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<DateTime>> Get()
        {
            return _context.WeeklySummaries.Select(ws => ws.WeekEnding);
        }
    }
}
