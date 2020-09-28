using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Nobreak.Context.Entities;
using Nobreak.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nobreak.Services
{
    public class NobreakLogic : INobreakProvider
    {
        private static readonly string RECENT_VALUES_ENTRY = nameof(RECENT_VALUES_ENTRY);
        private static readonly string UPTIME_ENTRY = nameof(UPTIME_ENTRY);

        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;

        public NobreakLogic(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task<UptimeReport> GetUptimeReportAsync() =>
            await _memoryCache.GetWithServiceAsync<UptimeReport, NobreakContext>(_serviceProvider, UPTIME_ENTRY, UptimeReport.Calculate, TimeSpan.FromSeconds(2));

        public async Task<List<NobreakState>> GetRecentValuesAsync() =>
            await _memoryCache.GetWithServiceAsync<List<NobreakState>, NobreakContext>(_serviceProvider, RECENT_VALUES_ENTRY, async context =>
            {
                var since = DateTime.Now - TimeSpan.FromDays(1);
                return await context.NobreakStates
                    .AsNoTracking()
                    .Where(s => (s.Id % 300 == 0 || s.Id > context.NobreakStates.Max(ss => ss.Id) - 30) && s.Timestamp >= since)
                    .OrderByDescending(s => s.Timestamp)
                    .ToListAsync();
            }, TimeSpan.FromSeconds(2));

        public async Task GetAllValuesAsync(Stream writeTo, string fileName)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<NobreakContext>();
            using var zip = new ZipArchive(writeTo, ZipArchiveMode.Create, false);
            var entryItem = zip.CreateEntry(fileName, CompressionLevel.Optimal);
            using var entryStream = entryItem.Open();
            await JsonSerializer.SerializeAsync(entryStream, context.NobreakStates.AsNoTracking());
        }

        public void ClearCache()
        {
            _memoryCache.Remove(RECENT_VALUES_ENTRY);
            _memoryCache.Remove(UPTIME_ENTRY);
        }

        public async Task<NobreakStateChange> ToggleOnPurposeAsync(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<NobreakContext>();
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
