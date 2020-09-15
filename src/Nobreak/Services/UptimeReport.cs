using Nobreak.Context.Entities;
using Nobreak.Entities;
using Nobreak.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Services
{
    public class UptimeReport
    {
        public List<TimeSpan> RelevantTimespans { get; set; }

        public List<NobreakState.PowerStates> PossibleStates { get; set; }

        public List<NobreakStateChange> RawData { get; set; }

        public List<UptimeInInterval> UptimePerIntervals { get; set; }

        public DateTime CalculatedOn { get; set; }

        public UptimeReport()
        {
            CalculatedOn = DateTime.Now;
            RelevantTimespans = new List<TimeSpan>
                {
                    TimeSpan.FromHours(5),
                    TimeSpan.FromDays(1),
                    TimeSpan.FromDays(7),
                    TimeSpan.FromDays(365.25/12),
                    TimeSpan.FromDays(365.25/4),
                    TimeSpan.FromDays(365.25/2),
                    TimeSpan.FromDays(365.25),
                    TimeSpan.FromDays(365.25 * 10)
                };
            PossibleStates = Enum.GetValues(typeof(NobreakState.PowerStates)).Cast<NobreakState.PowerStates>().ToList();
        }

        public UptimeReport CalculateDurations()
        {
            RawData = RawData.OrderByDescending(r => r.NobreakState.Timestamp).ToList();
            for (int i = 0; i < RawData.Count; i++)
                RawData[i].Duration = (i > 0 ? RawData[i - 1].NobreakState.Timestamp : CalculatedOn) - RawData[i].NobreakState.Timestamp;
            return this;
        }

        public UptimeReport CalculateUptimePerIntervals()
        {
            UptimePerIntervals = RelevantTimespans.Select(relevantTimespan =>
            {
                var since = CalculatedOn - relevantTimespan;
                var events = RawData.Where(s => s.NobreakState.Timestamp >= since).ToList();
                var rightBefore = RawData.SkipWhile(s => s.NobreakState.Timestamp >= (events.LastOrDefault()?.NobreakState.Timestamp ?? DateTime.Now)).FirstOrDefault();
                if (rightBefore != null)
                    events.Add(rightBefore);
                var sumPerState = PossibleStates.Select(s => new KeyValuePair<NobreakState.PowerStates, TimeSpan>(s, TimeSpan.Zero)).ToDictionary();
                for (int i = 0; i < events.Count; i++)
                    sumPerState[events[i].PowerState] += events[i].NobreakState.Timestamp < since
                        ? events[i].Duration - (CalculatedOn - relevantTimespan - events[i].NobreakState.Timestamp)
                        : events[i].Duration;
                var sum = TimeSpan.Zero;
                foreach (var state in sumPerState)
                    sum += state.Value;

                return new UptimeInInterval
                {
                    UptimeStates = sumPerState.Select(stateSum => new UptimeInInterval.UptimeState
                    {
                        ShareTimespan = stateSum.Value,
                        SharePercentage = (stateSum.Value / sum) * 100,
                        PowerState = stateSum.Key
                    }).ToList(),
                    Since = since,
                    TimeSpan = relevantTimespan
                };
            }).ToList();
            return this;
        }

        public static async Task<UptimeReport> Calculate(NobreakContext context)
        {
            var report = new UptimeReport
            {
                RawData = await context.NobreakStateChanges.AsNoTracking().OrderByDescending(s => s.NobreakState.Timestamp).Include(s => s.NobreakState).ToListAsync()
            }
            .CalculateDurations()
            .CalculateUptimePerIntervals();
            return report;
        }

        public class UptimeInInterval
        {
            public DateTime Since { get; set; }
            
            [JsonConverter(typeof(TimespanConverter))]
            public TimeSpan TimeSpan { get; set; }

            public List<UptimeState> UptimeStates { get; set; }

            public class UptimeState
            {
                public NobreakState.PowerStates PowerState { get; set; }
                
                [JsonConverter(typeof(TimespanConverter))]
                public TimeSpan ShareTimespan { get; set; }
                
                public double SharePercentage { get; set; }

                public override string ToString() =>
                    $"Modo {PowerState} durante {ShareTimespan.Format()} ({SharePercentage}% do total)";
            }

            public override string ToString() =>
                $"Entre {Since} até {Since + TimeSpan}: {string.Join("; ", UptimeStates)}";
        }

        public override string ToString() =>
            string.Join("\n", UptimePerIntervals ?? new List<UptimeInInterval>());
    }
}