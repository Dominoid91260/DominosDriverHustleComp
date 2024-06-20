﻿using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("{weekEnding}")]
        public async Task<IEnumerable<ReportRecord>> GetSingle(DateTime weekEnding)
        {
            var weeklySummary = await _context.WeeklySummaries
                .Include(ws => ws.DeliverySummaries)
                .ThenInclude(ds => ds.Driver)
                .FirstOrDefaultAsync(ws => ws.WeekEnding == weekEnding);

            if (weeklySummary is null)
                return [];

            return weeklySummary.DeliverySummaries.Select(ds => new ReportRecord
            {
                Name = ds.Driver.FirstName + " " + ds.Driver.LastName,
                AvgOut = ds.AvgHustleOut,
                AvgIn = ds.AvgHustleIn
            });
        }
    }
}
