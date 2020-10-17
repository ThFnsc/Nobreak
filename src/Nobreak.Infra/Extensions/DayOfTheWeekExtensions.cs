using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class DayOfTheWeekExtensions
    {
        public static bool IsWeekend(this DayOfWeek input) =>
            input == DayOfWeek.Sunday || input == DayOfWeek.Saturday;
    }
}
