using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class TimeSpanExtensions
    {
        private static string ShowIfGreaterThanZero(this int input, string suffix) =>
            input > 0
                ? $"{input}{suffix}"
                : string.Empty;
        public static string Format(this TimeSpan span, bool seconds = false) =>
            span.TotalSeconds< 0 
                ? $"-{span.Multiply(-1).Format(seconds)}"
                : (((int)span.TotalDays).ShowIfGreaterThanZero("d ") +
                span.Hours.ShowIfGreaterThanZero("h ") +
                span.Minutes.ShowIfGreaterThanZero("m ") +
                (seconds ? $"{span.Seconds}s" : string.Empty)).Trim();

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
                        "d" => (Func<int, TimeSpan>)(i => TimeSpan.FromDays(i)),
                        "h" => i => TimeSpan.FromHours(i),
                        "m" => i => TimeSpan.FromMinutes(i),
                        "s" => i => TimeSpan.FromSeconds(i),
                        _ => throw new Exception()
                    }).Invoke(int.Parse(part.Groups[1].Value)));
                return time;
            }catch(Exception)
            {
                return null;
            }
        }
    }
}
