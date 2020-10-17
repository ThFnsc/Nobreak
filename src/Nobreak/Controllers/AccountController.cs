using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Nobreak.Entities;
using Nobreak.Models;
using Nobreak.Services;
using Nobreak.Services.ReCAPTCHA;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nobreak.Context.Entities;

namespace Nobreak.Controllers
{
    public class AccountController : Controller
    {
        private readonly IReCaptchaValidator _reCaptchaValidator;
        private readonly NobreakContext _context;

        public AccountController(IReCaptchaValidator reCaptchaValidator, NobreakContext context)
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
                    : (IActionResult) Redirect(returnUrl)
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
                    _context.Accounts.Add(new Account
                    {
                        Email = model.Email,
                        PasswordHash = model.Password,
                        Name = model.Email,
                    });
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
