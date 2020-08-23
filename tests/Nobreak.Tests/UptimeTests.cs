using Nobreak.Context.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Nobreak.Services;

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
                RawData = new List<NobreakStateChange>
                {
                    new NobreakStateChange
                    {
                        NobreakState=new NobreakState
                        {
                            PowerState=NobreakState.PowerStates.Grid,
                            Timestamp=now-TimeSpan.FromHours(2)
                        }
                    },
                    new NobreakStateChange
                    {
                        NobreakState = new NobreakState
                        {
                            PowerState=NobreakState.PowerStates.Battery,
                            Timestamp=now-TimeSpan.FromHours(1)
                        }
                    }
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
