using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nobreak.Infra.Context;

namespace Nobreak.Configuration
{
    public static class ContextConfigs
    {
        public static IServiceCollection AddContextConfigs(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<IDbContext, DataContext>(optionsBuilder =>
            {
                optionsBuilder.UseMySQL(configuration.GetConnectionString("Default"));
                if (env.IsDevelopment())
                    optionsBuilder.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}
