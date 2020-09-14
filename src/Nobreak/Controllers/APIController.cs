using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nobreak.Entities;
using Nobreak.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Nobreak.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly CachedInfos _cachedInfos;
        private readonly NobreakContext _context;

        public APIController(CachedInfos cachedInfos, NobreakContext context)
        {
            _cachedInfos = cachedInfos;
            _context = context;
        }

        [HttpGet("RecentValues")]
        public async Task<IActionResult> RecentValues() =>
            Content(await _cachedInfos.GetAsync<string>(CachedInfos.ShortTermNobreakStatesJsonIdentifier), "application/json");

        [HttpGet("AllValues")]
        public async Task<IActionResult> AllValues() =>
            Content(await _cachedInfos.GetAsync<string>(CachedInfos.LongTermNobreakStatesJsonIdentifier), "application/json");

        [HttpGet("Uptime")]
        public async Task<IActionResult> Uptime() =>
            Content(await _cachedInfos.GetAsync<string>(CachedInfos.UptimeReportJsonIdentifier), "application/json");

        [HttpGet("ToggleOnPurpose/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleOnPurpose(int id)
        {
            var evt = await _context.NobreakStateChanges.SingleAsync(s => s.Id == id);
            evt.OnPurpose = !evt.OnPurpose;
            await _context.SaveChangesAsync();
            _cachedInfos.Invalidate(CachedInfos.UptimeReportIdentifier);
            _cachedInfos.Invalidate(CachedInfos.UptimeReportJsonIdentifier);
            return Ok();
        }
    }
}
