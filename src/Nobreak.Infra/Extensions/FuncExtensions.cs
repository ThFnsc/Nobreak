using System.Linq;

namespace System
{
    public static class FuncExtensions
    {
        public static string[] Repeat(this Func<string> func, int times) =>
            new string[times].Select(v => func()).ToArray();
    }
}
