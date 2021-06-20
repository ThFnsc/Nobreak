using Blazored.Modal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nobreak.Infra;

namespace Nobreak.Configuration
{
    public static class GeneralConfigs
    {
        public static IServiceCollection AddGeneralConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(options =>
                options.AllowSynchronousIO = true);
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
