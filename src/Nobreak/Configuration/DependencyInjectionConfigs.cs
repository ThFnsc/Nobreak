using Microsoft.Extensions.DependencyInjection;
using Nobreak.Infra.Services;
using Nobreak.Infra.Services.Serial;

namespace Nobreak.Configuration
{
    public static class DependencyInjectionConfigs
    {
        public static IServiceCollection AddDependencyInjectionConfigs(this IServiceCollection services)
        {
            services.AddSingleton<INobreakProvider, NobreakLogic>();
            services.AddSingleton<NobreakSerialMonitor>();
            services.AddHostedService(provider => provider.GetService<NobreakSerialMonitor>());
            return services;
        }
    }
}
