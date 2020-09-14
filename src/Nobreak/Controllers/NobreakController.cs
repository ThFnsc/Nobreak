using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nobreak.Entities;
using Nobreak.Services;
using Nobreak.Services.Serial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Nobreak.Controllers
{
    public class NobreakController : Controller
    {
        private readonly NobreakSerialMonitor _nobreak;
        private readonly NobreakContext _context;
        private readonly IMemoryCache _memoryCache;

        public NobreakController(NobreakSerialMonitor nobreak, NobreakContext context, IMemoryCache memoryCache)
        {
            _nobreak = nobreak;
            _context = context;
            _memoryCache = memoryCache;
        }

        public IActionResult Index() =>
            RedirectToAction("Events");

        public IActionResult Recent()=>
            View();

        public IActionResult Events() =>
            View();

        [Authorize]
        public async Task<IActionResult> ToggleOnPurpose(int id)
        {
            await new APIController(_context, _memoryCache).ToggleOnPurpose(id);
            return RedirectToAction("Events");
        }
    }
}
