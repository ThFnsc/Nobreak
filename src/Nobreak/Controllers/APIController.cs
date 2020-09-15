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
using Microsoft.Extensions.Caching.Memory;
using System.IO;

namespace Nobreak.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private static readonly string RECENT_VALUES_JSON_ENTRY = nameof(RECENT_VALUES_JSON_ENTRY);
        private static readonly string UPTIME_JSON_ENTRY = nameof(UPTIME_JSON_ENTRY);
        private static readonly string ALL_VALUES_ENTRY = nameof(ALL_VALUES_ENTRY);

        private readonly NobreakContext _context;
        private readonly IMemoryCache _memoryCache;

        public APIController(NobreakContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        [HttpGet("RecentValues")]
        public async Task<IActionResult> RecentValues() =>
            Ok(await _memoryCache.GetAsync(RECENT_VALUES_JSON_ENTRY, async () =>
            {
                var since = DateTime.Now - TimeSpan.FromDays(1);
                return (await _context.NobreakStates.AsNoTracking().Where(s => (s.Id % 300 == 0 || s.Id > _context.NobreakStates.Max(ss => ss.Id) - 30) && s.Timestamp >= since).OrderByDescending(s => s.Timestamp).ToListAsync()).ToJSON();
            }, TimeSpan.FromSeconds(2)));

        [HttpGet("AllValues")]
        public async Task<IActionResult> AllValues() =>
            File(await _memoryCache.GetAsync(ALL_VALUES_ENTRY, () =>
            {
                using var stream = new JsonStream(_context.NobreakStates.AsNoTracking()).CompressAsFile($"{DateTime.Now.ToISOString()}-dump.json");
                return Task.FromResult(stream.ToArray());
            }, TimeSpan.FromHours(1)), "text/plain;charset=utf-8", $"dump.zip");

        [HttpGet("Uptime")]
        public async Task<IActionResult> Uptime() =>
            Ok((await _memoryCache.GetAsync(UPTIME_JSON_ENTRY, async () => await UptimeReport.Calculate(_context), TimeSpan.FromSeconds(2))).ToJSON());

        [HttpGet("ToggleOnPurpose/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleOnPurpose(int id)
        {
            var evt = await _context.NobreakStateChanges.SingleAsync(s => s.Id == id);
            evt.OnPurpose = !evt.OnPurpose;
            await _context.SaveChangesAsync();
            _memoryCache.Remove(RECENT_VALUES_JSON_ENTRY);
            _memoryCache.Remove(UPTIME_JSON_ENTRY);
            return Ok();
        }
    }
}
