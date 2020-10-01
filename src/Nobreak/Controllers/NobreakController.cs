using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nobreak.Services;
using Microsoft.AspNetCore.Mvc;
using Nobreak.Models;
using System.Web;
using System.Net.Http.Headers;
using Nobreak.Services.ReCAPTCHA;

namespace Nobreak.Controllers
{
    public class NobreakController : Controller
    {
        private readonly INobreakProvider _nobreakProvider;

        public NobreakController(INobreakProvider nobreakProvider)
        {
            _nobreakProvider = nobreakProvider;
        }

        public IActionResult Index() =>
            RedirectToAction("Events");

        public IActionResult Recent()=>
            View();

        public IActionResult Events() =>
            View();

        public async Task<IActionResult> _EventsTable()
        {
            var uptime = await _nobreakProvider.GetUptimeReportAsync();
            return PartialView(uptime);
        }

        public async Task<IActionResult> _RecentValuesTable()
        {
            var recentValues = await _nobreakProvider.GetRecentValuesAsync();
            return PartialView(recentValues);
        }

        [HttpGet]
        public IActionResult DownloadAllValues() =>
            View();

        [HttpPost]
        [ReCaptchaChallenge(InvalidTokenErrorMessage = "Não foi possível confirmar que você não é um robô. Tente novamente, por favor 🤖")]
        public IActionResult DownloadAllValues(DownloadAllValuesViewModel model)
        {
            if (ModelState.IsValid)
                return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), 
                    $"{HttpUtility.UrlEncode(DateTime.Now.ToISOString())}-dump.zip", async (stream, _) =>
                        await _nobreakProvider.GetAllValuesAsync(stream));
            return View(model);
        }
    }
}
