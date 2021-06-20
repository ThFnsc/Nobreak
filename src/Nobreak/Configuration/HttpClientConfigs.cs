using Microsoft.Extensions.DependencyInjection;
using Nobreak.Infra.Services.ReCaptcha;
using System;

namespace Nobreak.Configuration
{
    public static class HttpClientConfigs
    {
        public static IServiceCollection AddHttpClientConfigs(this IServiceCollection services)
        {
            services.AddHttpClient<IReCaptchaValidator, ReCaptchaClient>(client =>
                client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/"));
            return services;
        }
    }
}
