using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nobreak.Infra;
using Nobreak.Infra.Context;
using Nobreak.Infra.Services;
using Nobreak.Infra.Services.ReCaptcha;
using Nobreak.Infra.Services.Serial;
using System;

namespace Nobreak
{
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _currentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
                options.AllowSynchronousIO = true);

            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddMemoryCache();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddSingleton<INobreakProvider, NobreakLogic>();
            services.AddSingleton<NobreakSerialMonitor>();
            services.AddHostedService(provider => provider.GetService<NobreakSerialMonitor>());

            services.AddDbContext<IDbContext, DataContext>(optionsBuilder =>
            {
                optionsBuilder.UseMySQL(Configuration.GetConnectionString("Default"));
                if (_currentEnvironment.IsDevelopment())
                    optionsBuilder.EnableSensitiveDataLogging();
            });

            services.AddHttpClient<IReCaptchaValidator, ReCaptchaClient>(client =>
                client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = "Nobreak";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IDbContext context,
            IOptions<AppSettings> settings)
        {
            if (settings.Value.RunMigrationsOnStartup)
                context.Migrate();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
