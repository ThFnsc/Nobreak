using System.Globalization;

namespace System
{
    public static class DateTimeExtensions
    {
        public static string FormatDate(this DateTime input) =>
            input.ToString("ddd dd'/'MM'/'yyyy", new CultureInfo("pt-BR"));

        public static string ToISOString(this DateTime input) =>
            input.ToString("o");

        public static string FormatTime(this DateTime input, bool includeSeconds = false) =>
            input.ToString($"hh:mm{(includeSeconds ? ":ss" : "")} tt");

        public static string Format(this DateTime input, bool includeSeconds = false) =>
            $"{input.FormatDate()} {input.FormatTime(includeSeconds)}";
    }
}
