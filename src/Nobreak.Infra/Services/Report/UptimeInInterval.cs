using System;
using System.Collections.Generic;

namespace Nobreak.Infra.Services.Report
{
    public class UptimeInInterval
    {
        public DateTime Since { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public List<UptimeState> UptimeStates { get; set; }

        public override string ToString() =>
            $"Entre {Since} até {Since + TimeSpan}: {string.Join("; ", UptimeStates)}";
    }
}