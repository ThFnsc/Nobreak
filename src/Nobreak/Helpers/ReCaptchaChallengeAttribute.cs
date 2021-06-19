using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Nobreak.Infra.Services.ReCaptcha;
using System;
using System.Threading.Tasks;

namespace Nobreak.Helpers
{
    public class ReCaptchaChallengeAttribute : ActionFilterAttribute
    {
        public string InvalidTokenErrorMessage { get; set; } = "Could not prove you're not a robot. Please try again";

        public string MissingTokenErrorMessage { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetRequiredService<IReCaptchaValidator>();

            if (!context.ActionArguments.TryGetValue("model", out var model))
                throw new Exception("Missing model");
            if (!(model is IReCaptchaRequired reCaptchaModel))
                throw new Exception("Model does not implement " + nameof(IReCaptchaRequired));
            if (string.IsNullOrWhiteSpace(reCaptchaModel.ReCaptchaToken))
                context.ModelState.AddModelError(nameof(reCaptchaModel.ReCaptchaToken), MissingTokenErrorMessage ?? InvalidTokenErrorMessage);
            else if (!await validator.Passed(reCaptchaModel.ReCaptchaToken))
                context.ModelState.AddModelError(nameof(reCaptchaModel.ReCaptchaToken), InvalidTokenErrorMessage);

            await next();
        }
    }
}
