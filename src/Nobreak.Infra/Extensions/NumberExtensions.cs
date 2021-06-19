namespace System
{
    public static class NumberExtensions
    {
        public static DateTime AsUTCTimeStamp(this int input) =>
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(input);

        public static int Mod(this int a, int b) =>
            a - (int) Math.Floor((double) a / b) * b;

        public static float Map(this float x, float in_min, float in_max, float out_min, float out_max, bool cutoff = false)
        {
            var y = (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
            if (!cutoff)
                return y;
            if (y > out_max)
                return out_max;
            if (y < out_min)
                return out_min;
            return y;
        }
    }
}
