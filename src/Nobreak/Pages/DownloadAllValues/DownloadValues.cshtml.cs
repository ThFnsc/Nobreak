using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Nobreak.Helpers;
using Nobreak.Infra.Services;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Nobreak.Pages
{
    public class DownloadValuesModel : PageModel
    {
        private readonly INobreakProvider _nobreakProvider;
        private readonly IDistributedCache _cache;

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public DownloadValuesModel(
            INobreakProvider nobreakProvider,
            IDistributedCache cache)
        {
            _nobreakProvider = nobreakProvider;
            _cache = cache;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var tokenInCache = await _cache.GetStringAsync(Token);
            if (tokenInCache is not null && tokenInCache == Token)
            {
                await _cache.RemoveAsync(Token);
                return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"),
                    $"{HttpUtility.UrlEncode(DateTime.Now.ToISOString())}-dump.zip", async (stream, _) =>
                        await _nobreakProvider.GetAllValuesAsync(stream));
            }
            return Unauthorized();
        }
    }
}