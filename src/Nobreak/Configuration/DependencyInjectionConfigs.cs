using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nobreak.Infra.Services;
using Nobreak.Infra.Services.Serial;

namespace Nobreak.Configuration
{
    public static class DependencyInjectionConfigs
    {
        public static IServiceCollection AddDependencyInjectionConfigs(this IServiceCollection services)
        {
            services.AddSingleton<INobreakProvider, NobreakLogic>();

            services.AddHostedService(provider =>
            {
                var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;
                return new NobreakSerialMonitor(
                    logger: provider.GetRequiredService<ILogger<NobreakSerialMonitor>>(),
                    serviceProvider: provider,
                    serialPortOverride: appSettings.SerialPort,
                    bauldRateOverride: appSettings.BauldRate);
            });

            return services;
        }
    }
}
