using System.Globalization;

namespace System
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime date) =>
            Convert.ToInt64((date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

        public static TimeSpan BetweenThenUTCAndNow(this DateTime input) =>
            DateTime.UtcNow - input;

        public static string FormatDate(this DateTime input) =>
            input.ToString("ddd dd'/'MM'/'yyyy", new CultureInfo("pt-BR"));

        public static string ToISOString(this DateTime input) =>
            input.ToString("o");

        public static string FormatTime(this DateTime input, bool includeSeconds = false) =>
            input.ToString($"hh:mm{(includeSeconds ? ":ss" : "")} tt");

        public static string Format(this DateTime input, bool includeSeconds = false) =>
            $"{input.FormatDate()} {input.FormatTime(includeSeconds)}";

        public static DateTime Next(this DateTime input, DayOfWeek dayOfWeek) =>
            input.Next(d => d.DayOfWeek == dayOfWeek);

        public static DateTime Last(this DateTime input, DayOfWeek dayOfWeek) =>
            input.Next(d => d.DayOfWeek == dayOfWeek, true);

        public static DateTime Last(this DateTime input, Predicate<DateTime> predicate) =>
            input.Next(predicate, true);

        public static DateTime Next(this DateTime input, Predicate<DateTime> predicate, bool backwards = false)
        {
            do input = input.AddDays(backwards ? -1 : 1);
            while (!predicate(input));
            return input;
        }
    }
}
