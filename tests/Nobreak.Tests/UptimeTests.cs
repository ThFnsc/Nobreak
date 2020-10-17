using Nobreak.Context.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Nobreak.Models;
using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;

namespace Nobreak.Tests
{
    [TestClass]
    public class UptimeTests
    {
        [TestMethod]
        public void UptimeCalculationTest()
        {
            var now = DateTime.Now;
            var uptime = new UptimeReport
            {
                CalculatedOn = now,
                StateChanges = new List<NobreakStateChange>
                {
                    new NobreakStateChange(new NobreakState(PowerStates.Grid,now-TimeSpan.FromHours(2))),
                    new NobreakStateChange(new NobreakState(PowerStates.Battery,now-TimeSpan.FromHours(1)))
                }
            }
            .CalculateDurations()
            .CalculateUptimePerIntervals();
            foreach (var interval in uptime.UptimePerIntervals)
                foreach (var state in interval.UptimeStates)
                    Assert.AreEqual(state.SharePercentage, 50);
        }
    }
}