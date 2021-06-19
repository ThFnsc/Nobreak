using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nobreak.Context.Entities;
using Nobreak.Helpers;
using Nobreak.Infra.Context;
using Nobreak.Infra.Services.ReCaptcha;
using Nobreak.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nobreak.Controllers
{
    public class AccountController : Controller
    {
        private readonly IReCaptchaValidator _reCaptchaValidator;
        private readonly IDbContext _context;

        public AccountController(IReCaptchaValidator reCaptchaValidator, IDbContext context)
        {
            _reCaptchaValidator = reCaptchaValidator;
            _context = context;
        }

        public IActionResult Index() =>
            RedirectToAction("Login");

        [HttpGet]
        public IActionResult Login(string returnUrl = null) =>
            User.Identity.IsAuthenticated
                ? string.IsNullOrWhiteSpace(returnUrl)
                    ? RedirectToAction("Index", "Home")
                    : Redirect(returnUrl)
                : View();

        [HttpPost]
        [ReCaptchaChallenge(InvalidTokenErrorMessage = "Não foi possível confirmar que você não é um robô. Tente novamente, por favor 🤖")]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Accounts.SingleOrDefaultAsync(usr => usr.Email.ToLower() == model.Email.ToLower());
                if (user != null && model.Password == user.PasswordHash)
                {
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(user.Claims(), CookieAuthenticationDefaults.AuthenticationScheme));
                    await HttpContext.SignInAsync(principal);
                    HttpContext.User = principal;
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else if (await _context.Accounts.CountAsync() == 0)
                {
                    _context.Add(new Account(model.Email, model.Email, model.Password));
                    await _context.SaveChangesAsync();
                    return await Login(model, returnUrl);
                }
                else
                    ModelState.AddModelError(nameof(Login), "Combinação de e-mail e senha incorreta");
            }
            return View(model);
        }
    }
}
