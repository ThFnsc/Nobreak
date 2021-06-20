namespace System
{
    public static class NumberExtensions
    {
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
