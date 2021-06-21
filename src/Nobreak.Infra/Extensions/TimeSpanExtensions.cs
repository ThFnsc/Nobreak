using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class TimeSpanExtensions
    {
        private static KeyValuePair<string, double>[] _timeUnits;

        public static string Format(this TimeSpan span, int limitUnits = 2)
        {
            if (_timeUnits == null)
            {
                _timeUnits = new Dictionary<string, double>
                {
                    { "s", 1 },
                    { "m", 60 },
                    { "h", 60 },
                    { "d", 24 },
                    { "w", 7 },
                    { "mo", 365.25/12/7 },
                    { "y", 12 },
                    { "dec", 10 },
                    { "c", 10 }
                }.ToArray();
                for (var i = 1; i < _timeUnits.Length; i++)
                    _timeUnits[i] = new KeyValuePair<string, double>(_timeUnits[i].Key, _timeUnits[i].Value * _timeUnits[i - 1].Value);
            }

            var seconds = span.TotalSeconds;

            if (seconds < 0)
                return $"-({span.Multiply(-1).Format()})";

            var values = new List<string>();
            for (var i = _timeUnits.Length - 1; i >= 0; i--)
            {
                var result = Math.Floor(seconds / _timeUnits[i].Value);
                if (result >= 1)
                {
                    seconds %= _timeUnits[i].Value;
                    values.Add($"{result}{_timeUnits[i].Key}");
                }
            }
            if (values.Count == 0)
                values.Add($"0{_timeUnits.First().Key}");
            return string.Join(" ", values.Take(limitUnits));
        }
    }
}
