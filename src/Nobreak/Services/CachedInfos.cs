using Nobreak.Context.Entities;
using Nobreak.Entities;
using Nobreak.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Services
{
    public class CachedInfos : CachedResources
    {
        public static readonly object UptimeReportIdentifier = nameof(UptimeReportIdentifier);
        public static readonly object UptimeReportJsonIdentifier = nameof(UptimeReportJsonIdentifier);
        public static readonly object ShortTermNobreakStatesIdentifier = nameof(ShortTermNobreakStatesIdentifier);
        public static readonly object LongTermNobreakStatesIdentifier = nameof(LongTermNobreakStatesIdentifier);
        public static readonly object ShortTermNobreakStatesJsonIdentifier = nameof(ShortTermNobreakStatesJsonIdentifier);
        public static readonly object LongTermNobreakStatesJsonIdentifier = nameof(LongTermNobreakStatesJsonIdentifier);

        public CachedInfos(IServiceProvider serviceProvider)
        {
            RegisterWithDisposibleService<UptimeReport, NobreakContext>(UptimeReport.Calculate, serviceProvider, TimeSpan.FromSeconds(2), UptimeReportIdentifier);

            RegisterWithDisposibleService<List<NobreakState>, NobreakContext>(async context =>
            {
                var since = DateTime.Now - TimeSpan.FromDays(1);
                return await context.NobreakStates.Where(s => (s.Id % 30 == 0 || s.Id > context.NobreakStates.Max(ss => ss.Id) - 30) && s.Timestamp >= since).OrderByDescending(s => s.Timestamp).ToListAsync();
            }, serviceProvider, TimeSpan.FromSeconds(2), ShortTermNobreakStatesIdentifier);

            RegisterWithDisposibleService<List<NobreakState>, NobreakContext>(async context =>
                await context.NobreakStates.OrderByDescending(s => s.Timestamp).ToListAsync(), serviceProvider, TimeSpan.FromHours(1), LongTermNobreakStatesIdentifier);


            Register(async () =>
                JsonConvert.SerializeObject(await GetAsync<UptimeReport>(UptimeReportIdentifier)), TimeSpan.FromSeconds(2), UptimeReportJsonIdentifier);

            Register(async () =>
                JsonConvert.SerializeObject(await GetAsync<List<NobreakState>>(ShortTermNobreakStatesIdentifier)), TimeSpan.FromSeconds(2), ShortTermNobreakStatesJsonIdentifier);

            Register(async () =>
                JsonConvert.SerializeObject(await GetAsync<List<NobreakState>>(LongTermNobreakStatesIdentifier)), TimeSpan.FromHours(1), LongTermNobreakStatesJsonIdentifier);
        }
    }
}
