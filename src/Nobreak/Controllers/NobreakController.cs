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

namespace Nobreak.Controllers
{
    public class NobreakController : Controller
    {
        private readonly NobreakSerialMonitor _nobreak;
        private readonly NobreakContext _context;
        private readonly CachedInfos _cachedInfos;

        public NobreakController(NobreakSerialMonitor nobreak, NobreakContext context, CachedInfos cachedInfos)
        {
            _nobreak = nobreak;
            _context = context;
            _cachedInfos = cachedInfos;
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
            await new APIController(_cachedInfos, _context).ToggleOnPurpose(id);
            return RedirectToAction("Events");
        }
    }
}
