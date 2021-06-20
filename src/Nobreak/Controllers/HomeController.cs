using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nobreak.Helpers;
using Nobreak.Infra.Services;
using System;
using System.Net.Http.Headers;
using System.Web;

namespace Nobreak.Controllers
{
    public class HomeController : Controller
    {
        private readonly INobreakProvider _nobreakProvider;
        private readonly IMemoryCache _cache;

        public HomeController(
            INobreakProvider nobreakProvider,
            IMemoryCache cache)
        {
            _nobreakProvider = nobreakProvider;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult DownloadAllValues(string token)
        {
            if (_cache.TryGetValue(token, out var inCache))
                if (token == inCache.ToString())
                {
                    _cache.Remove(token);
                    return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"),
                        $"{HttpUtility.UrlEncode(DateTime.Now.ToISOString())}-dump.zip", async (stream, _) =>
                            await _nobreakProvider.GetAllValuesAsync(stream));
                }
            return Unauthorized();
        }
    }
}
