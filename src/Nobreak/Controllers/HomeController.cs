using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nobreak.Helpers;
using Nobreak.Infra.Services;
using Nobreak.Models;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Web;

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
