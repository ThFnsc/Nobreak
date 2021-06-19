using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace Nobreak.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder applicationBuilder, ILogger logger, LogLevel logLevel = LogLevel.Trace) =>
            applicationBuilder.Use(async (context, next) =>
            {
                var started = Stopwatch.StartNew();
                var user = context.User.Identity.IsAuthenticated ? context.User.FindFirst(ClaimTypes.Name).Value : "Anonymous user";
                logger.Log(logLevel, "{User} made a {Scheme} {Method} request to {Path} from the IP {IPAddress}", user, context.Request.Scheme, context.Request.Method, context.Request.Path.Value, context.Connection.RemoteIpAddress.ToString());
                await next();
                logger.Log(logLevel, "{User}'s {Method} request to {Path} ended with status code {StatusCode}. Took {TotalSeconds}s", user, context.Request.Method, context.Request.Path.Value, context.Response.StatusCode, started.Elapsed.TotalSeconds);
            });
    }
}