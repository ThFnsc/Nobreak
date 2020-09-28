using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nobreak.Tests
{
    [TestClass]
    public class TimeSpanExtensionsTests
    {
        [TestMethod]
        public void ShouldFormatCorrectly()
        {
            var values = new Dictionary<TimeSpan, string>
            {
                {TimeSpan.Zero, "0s" },
                {TimeSpan.FromSeconds(-1), "-(1s)" },
                {TimeSpan.FromDays(1), "1d" },
                {TimeSpan.FromDays(7), "1w" },
                {TimeSpan.FromDays(8), "1w 1d" },
                {TimeSpan.FromDays(365.25), "1y" },
                {TimeSpan.FromDays(365.25)
                    .Add(TimeSpan.FromDays(8))
                    .Add(TimeSpan.FromHours(12))
                    .Add(TimeSpan.FromMinutes(54.5)), "1y 1w 1d 12h 54m 30s" },
            };

            var converted = values.Select(v => new KeyValuePair<string, string>(v.Key.Format(), v.Value)).ToList();
            foreach (var kvp in converted)
                Assert.AreEqual(kvp.Key, kvp.Value);
        }
    }
}
