using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Nobreak.Entities;
using Nobreak.Services;
using Nobreak.Services.ReCAPTCHA;
using Nobreak.Services.Serial;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                settings.Formatting = Formatting.Indented;
                return settings;
            };

            services.AddControllersWithViews();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            services.AddSingleton<NobreakSerialMonitor>();
            services.AddHostedService(provider => provider.GetService<NobreakSerialMonitor>());

            services.AddDbContext<NobreakContext>(optionsBuilder =>
                optionsBuilder.UseMySQL(Configuration.GetConnectionString("Default")));

            services.AddSingleton<CachedInfos>();

            if (_currentEnvironment.IsDevelopment())
            {
                services.AddSingleton<IReCaptchaValidator, ReCaptchaAllowAll>();
            }
            else
            {
                services.AddSingleton<IReCaptchaValidator, ReCAPTCHAClient>();
            }

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = "Nobreak";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                DateTime start = DateTime.Now;
                var user = context.User.Identity.IsAuthenticated ? context.User.FindFirst(ClaimTypes.Name).Value : "Anonymous user";
                logger.LogInformation($"{user} made a {context.Request.Scheme} {context.Request.Method} request to {context.Request.Path} from the IP {context.Request.Headers["X-Forwarded-For"]}");
                await next();
                logger.LogInformation($"{user}'s {context.Request.Method} request to {context.Request.Path} ended with status code {context.Response.StatusCode}. Took {(DateTime.Now - start).TotalSeconds}s");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
