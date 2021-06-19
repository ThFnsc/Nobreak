﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class TimeSpanExtensions
    {
        private static KeyValuePair<string, double>[] TIME_UNITS;

        private static string ShowIfGreaterThanZero(this int input, string suffix) =>
            input > 0
                ? $"{input}{suffix}"
                : string.Empty;

        public static string Format(this TimeSpan span)
        {
            if (TIME_UNITS == null)
            {
                TIME_UNITS = new Dictionary<string, double>
                {
                    { "s", 1 },
                    { "m", 60 },
                    { "h", 60 },
                    { "d", 24 },
                    { "w", 7 },
                    { "mo", (365.25/12)/7 },
                    { "y", 12 },
                    { "dec", 10 },
                    { "c", 10 }
                }.ToArray();
                for (var i = 1; i < TIME_UNITS.Length; i++)
                    TIME_UNITS[i] = new KeyValuePair<string, double>(TIME_UNITS[i].Key, TIME_UNITS[i].Value * TIME_UNITS[i - 1].Value);
            }

            var seconds = span.TotalSeconds;

            if (seconds < 0)
                return $"-({span.Multiply(-1).Format()})";

            var values = new List<string>();
            for (var i = TIME_UNITS.Length - 1; i >= 0; i--)
            {
                var result = Math.Floor(seconds / TIME_UNITS[i].Value);
                if (result >= 1)
                {
                    seconds %= TIME_UNITS[i].Value;
                    values.Add($"{result}{TIME_UNITS[i].Key}");
                }
            }
            if (values.Count == 0)
                values.Add($"0{TIME_UNITS.First().Key}");
            return string.Join(" ", values);
        }

        public static string ToAmPm(this TimeSpan input, bool seconds = false) =>
            $"{(input.Hours == 0 ? input.Add(TimeSpan.FromHours(12)) : (input.Hours > 12 ? input.Subtract(TimeSpan.FromHours(12)) : input)).ToString($"hh\\:mm{(seconds ? "\\:ss" : "")}")} {(input.Hours < 12 ? "am" : "pm")}";

        public static bool IsWithin(this TimeSpan input, TimeSpan start, TimeSpan end) =>
            start > end
                ? start <= input || input < end
                : start <= input && input < end;

        public static DateTime ToDateTime(this TimeSpan input) =>
            new DateTime(0) + input;

        public static TimeSpan? TryParseNatural(string input)
        {
            try
            {
                input = string.Join(string.Empty, input.Where(char.IsLetterOrDigit));
                var parts = Regex.Matches(input, @"(\d*)(d|h|m|s)");
                if (parts.Count == 0)
                    return null;
                var time = new TimeSpan(0);
                foreach (Match part in parts)
                    time = time.Add((part.Groups[2].Value switch
                    {
                        "d" => (Func<int, TimeSpan>) (i => TimeSpan.FromDays(i)),
                        "h" => i => TimeSpan.FromHours(i),
                        "m" => i => TimeSpan.FromMinutes(i),
                        "s" => i => TimeSpan.FromSeconds(i),
                        _ => throw new Exception()
                    }).Invoke(int.Parse(part.Groups[1].Value)));
                return time;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
