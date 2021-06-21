using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Nobreak.Infra.Context;
using Nobreak.Infra.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nobreak.Pages
{
    public class AuthorizeModel : PageModel
    {
        private readonly IDbContext _context;
        private readonly IDistributedCache _cache;

        public AuthorizeModel(
            IDbContext context,
            IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = await _cache.GetStringAsync(Token);
            if (userId is null)
                return Unauthorized();
            await _cache.RemoveAsync(Token);
            var user = await _context.Accounts
                .OfId(int.Parse(userId))
                .SingleAsync();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(user.Claims(), CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(principal, new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.AddMonths(1)
            });

            HttpContext.User = principal;
            return !string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)
                ? Redirect(ReturnUrl)
                : Redirect("/");
        }
    }
}
