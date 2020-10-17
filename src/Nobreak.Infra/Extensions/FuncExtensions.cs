using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class FuncExtensions
    {
        public static string[] Repeat(this Func<string> func, int times) =>
            new string[times].Select(v => func()).ToArray();
    }
}
