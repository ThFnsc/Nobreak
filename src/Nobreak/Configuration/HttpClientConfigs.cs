using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nobreak.Infra.Services.ReCaptcha;
using System;

namespace Nobreak.Configuration
{
    public static class HttpClientConfigs
    {
        public static IServiceCollection AddHttpClientConfigs(this IServiceCollection services)
        {
            services.AddHttpClient<IReCaptchaValidator, ReCaptchaClient>((client, provider) =>
            {
                var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;
                client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
                return new ReCaptchaClient(
                    httpClient: client,
                    siteKey: appSettings.RecaptchaSiteKey,
                    secret: appSettings.RecaptchaSecret,
                    logger: provider.GetRequiredService<ILogger<ReCaptchaClient>>());
            });
            return services;
        }
    }
}
