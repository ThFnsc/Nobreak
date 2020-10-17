using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nobreak.Models;
using System.Web;
using System.Net.Http.Headers;
using System.Diagnostics;
using Nobreak.Helpers;
using Nobreak.Infra.Services;
using AutoMapper;

namespace Nobreak.Controllers
{
    public class HomeController : Controller
    {
        private readonly INobreakProvider _nobreakProvider;
        private readonly IMapper _mapper;

        public HomeController(INobreakProvider nobreakProvider, IMapper mapper)
        {
            _nobreakProvider = nobreakProvider;
            _mapper = mapper;
        }

        public IActionResult Index() =>
            RedirectToAction(nameof(Events));

        public IActionResult Recent() =>
            View();

        public IActionResult Events() =>
            View();

        public async Task<IActionResult> EventsTable()
        {
            var uptime = await _nobreakProvider.GetUptimeReportAsync();
            var model = _mapper.Map<UptimeReportViewModel>(uptime);
            return PartialView("_EventsTable", model);
        }

        public async Task<IActionResult> RecentValuesTable()
        {
            var recentValues = await _nobreakProvider.GetRecentValuesAsync();
            var model = new RecentValuesViewModel
            {
                NobreakStates = _mapper.Map<List<NobreakStateViewModel>>(recentValues)
            };
            return PartialView("_RecentValuesTable", model);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
