using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Nobreak.Context.Entities;
using Nobreak.Extensions;
using Nobreak.Infra.Context;
using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;
using System;
using System.Collections.Generic;
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
        private readonly IMemoryCache _memoryCache;

        public NobreakLogic(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task<UptimeReport> GetUptimeReportAsync() =>
            await _memoryCache.GetWithServiceAsync<UptimeReport, IDbContext>(_serviceProvider, _uptimeEntry, UptimeReport.Calculate, TimeSpan.FromSeconds(2));

        public async Task<List<NobreakState>> GetRecentValuesAsync() =>
            await _memoryCache.GetWithServiceAsync<List<NobreakState>, IDbContext>(_serviceProvider, _recentValuesEntry, async context =>
            {
                var since = DateTime.Now - TimeSpan.FromDays(1);
                return await context.NobreakStates
                    .AsNoTracking()
                    .Where(s => (s.Id % 300 == 0 || s.Id > context.NobreakStates.Max(ss => ss.Id) - 30) && s.Timestamp >= since)
                    .OrderByDescending(s => s.Timestamp)
                    .ToListAsync();
            }, TimeSpan.FromSeconds(2));

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
                jsonSettings.Converters.AddRange(new JsonStringEnumConverter(), new TimespanConverter());
                jsonSettings.IgnoreReadOnlyProperties = true;
                await JsonSerializer.SerializeAsync(changesStream, source(context), jsonSettings);
            }

            await SerializeTo($"changes.json", context => context.NobreakStateChanges.Include(s => s.NobreakState).AsNoTracking());
            await SerializeTo($"readings.json", context => context.NobreakStates.AsNoTracking());
        }

        public void ClearCache()
        {
            _memoryCache.Remove(_recentValuesEntry);
            _memoryCache.Remove(_uptimeEntry);
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
            ClearCache();
            return evt;
        }
    }
}
