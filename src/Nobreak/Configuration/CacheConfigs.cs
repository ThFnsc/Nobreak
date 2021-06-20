using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Nobreak.Configuration
{
    public static class CacheConfigs
    {
        public static IServiceCollection AddCacheConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfStr = configuration.GetConnectionString("Redis");
            using var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();

            if (string.IsNullOrWhiteSpace(redisConfStr))
            {
                logger.LogWarning("No Redis configuration string set. Using memory distributed cache.");
                services.AddDistributedMemoryCache();
            }
            else
            {
                var redisConfig = ConfigurationOptions.Parse(redisConfStr);
                services.AddStackExchangeRedisCache(options => options.ConfigurationOptions = redisConfig);
                logger.LogInformation("Using Redis as distributed cache");
            }

            services.AddMemoryCache();
            return services;
        }
    }
}
