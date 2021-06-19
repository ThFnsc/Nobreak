namespace System
{
    public static class DayOfTheWeekExtensions
    {
        public static bool IsWeekend(this DayOfWeek input) =>
            input == DayOfWeek.Sunday || input == DayOfWeek.Saturday;
    }
}
