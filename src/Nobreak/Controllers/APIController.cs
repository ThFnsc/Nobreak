using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nobreak.Context.Entities;
using Nobreak.Models;
using Nobreak.Infra.Services;
using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;

namespace Nobreak.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly INobreakProvider _nobreakProvider;

        public APIController(INobreakProvider nobreakProvider)
        {
            _nobreakProvider = nobreakProvider;
        }

        [HttpGet("RecentValues")]
        public async Task<List<NobreakState>> RecentValues() =>
            await _nobreakProvider.GetRecentValuesAsync();

        [HttpGet("Uptime")]
        public async Task<UptimeReport> Uptime() =>
            await _nobreakProvider.GetUptimeReportAsync();

        [HttpGet("ToggleOnPurpose/{id}")]
        [Authorize]
        public async Task<NobreakStateChange> ToggleOnPurpose(int id) =>
            await _nobreakProvider.ToggleOnPurposeAsync(id);
    }
}
