using Nobreak.Context.Entities;
using System;

namespace Nobreak.Infra.Services.Report
{
    public class UptimeState
    {
        public PowerStates PowerState { get; set; }

        public TimeSpan ShareTimespan { get; set; }

        public double SharePercentage { get; set; }

        public override string ToString() =>
            $"Modo {PowerState} durante {ShareTimespan.Format()} ({SharePercentage}% do total)";
    }
}