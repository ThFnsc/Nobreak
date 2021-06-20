using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace Nobreak.Configuration
{
    public static class AuthConfigs
    {
        public static IServiceCollection AddAuthConfigs(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => { });
            return services;
        }
    }
}
