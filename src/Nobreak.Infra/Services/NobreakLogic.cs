using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nobreak.Extensions;
using Nobreak.Infra.Context;
using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services
{
    public class NobreakLogic : INobreakProvider
    {
        private static readonly string _recentValuesEntry = nameof(_recentValuesEntry);
        private static readonly string _uptimeEntry = nameof(_uptimeEntry);

        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContext _context;

        public NobreakLogic(
            IServiceProvider serviceProvider,
            IDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public Task<UptimeReport> GetUptimeReportAsync() =>
            UptimeReport.Calculate(_context);

        public async Task GetAllValuesAsync(Stream writeTo)
        {
            using var zip = new ZipArchive(writeTo, ZipArchiveMode.Create, false);

            async Task SerializeTo<T>(string name, Func<IDbContext, IQueryable<T>> source)
            {
                var changesEntry = zip.CreateEntry(name, CompressionLevel.Optimal);
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<IDbContext>();
                using var changesStream = changesEntry.Open();
                var jsonSettings = new JsonSerializerOptions();
                jsonSettings.Converters.Add(new JsonStringEnumConverter());
                jsonSettings.Converters.Add(new TimespanConverter());
                jsonSettings.IgnoreReadOnlyProperties = true;
                await JsonSerializer.SerializeAsync(changesStream, source(context), jsonSettings);
            }

            await SerializeTo($"changes.json", context => context.NobreakStateChanges.Include(s => s.NobreakState).AsNoTracking());
            await SerializeTo($"readings.json", context => context.NobreakStates.AsNoTracking());
        }

        public async Task<NobreakStateChange> ToggleOnPurposeAsync(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var evt = await context.NobreakStateChanges
                .Include(s => s.NobreakState)
                .SingleAsync(s => s.Id == id);
            evt.OnPurpose = !evt.OnPurpose;
            await context.SaveChangesAsync();
            return evt;
        }
    }
}
