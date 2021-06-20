using Microsoft.EntityFrameworkCore;
using Nobreak.Context.Entities;
using Nobreak.Infra.Context;
using Nobreak.Infra.Context.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services.Report
{
    public class UptimeReport
    {
        public List<TimeSpan> RelevantTimespans { get; set; }

        public List<PowerStates> PossibleStates { get; set; }

        public List<NobreakStateChange> StateChanges { get; set; }

        public List<UptimeInInterval> UptimePerIntervals { get; set; }

        public DateTime CalculatedOn { get; set; }

        public UptimeReport()
        {
            CalculatedOn = DateTime.UtcNow;
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
            PossibleStates = Enum.GetValues(typeof(PowerStates)).Cast<PowerStates>().ToList();
        }

        public UptimeReport CalculateDurations()
        {
            StateChanges = StateChanges.OrderByDescending(r => r.NobreakState.Timestamp).ToList();
            for (var i = 0; i < StateChanges.Count; i++)
                StateChanges[i].Duration = (i > 0 ? StateChanges[i - 1].NobreakState.Timestamp : CalculatedOn) - StateChanges[i].NobreakState.Timestamp;
            return this;
        }

        public UptimeReport CalculateUptimePerIntervals()
        {
            UptimePerIntervals = RelevantTimespans.Select(relevantTimespan =>
            {
                var since = CalculatedOn - relevantTimespan;
                var events = StateChanges.Where(s => s.NobreakState.Timestamp >= since).ToList();
                var rightBefore = StateChanges.SkipWhile(s => s.NobreakState.Timestamp >= (events.LastOrDefault()?.NobreakState.Timestamp ?? DateTime.Now)).FirstOrDefault();
                if (rightBefore != null)
                    events.Add(rightBefore);
                var sumPerState = PossibleStates.Select(s => new KeyValuePair<PowerStates, TimeSpan>(s, TimeSpan.Zero)).ToDictionary();
                for (var i = 0; i < events.Count; i++)
                    sumPerState[events[i].PowerState] += events[i].NobreakState.Timestamp < since
                        ? events[i].Duration - (CalculatedOn - relevantTimespan - events[i].NobreakState.Timestamp)
                        : events[i].Duration;
                var sum = TimeSpan.Zero;
                foreach (var state in sumPerState)
                    sum += state.Value;

                return new UptimeInInterval
                {
                    UptimeStates = sumPerState.Select(stateSum => new UptimeState
                    {
                        ShareTimespan = stateSum.Value,
                        SharePercentage = stateSum.Value / sum * 100,
                        PowerState = stateSum.Key
                    }).ToList(),
                    Since = since,
                    TimeSpan = relevantTimespan
                };
            }).ToList();
            return this;
        }

        public static async Task<UptimeReport> Calculate(IDbContext context)
        {
            var report = new UptimeReport
            {
                StateChanges = await context.NobreakStateChanges.AsNoTracking().OrderByDescending(s => s.NobreakState.Timestamp).Include(s => s.NobreakState).ToListAsync()
            }
            .CalculateDurations()
            .CalculateUptimePerIntervals();
            return report;
        }

        public override string ToString() =>
            string.Join("\n", UptimePerIntervals ?? new List<UptimeInInterval>());
    }
}