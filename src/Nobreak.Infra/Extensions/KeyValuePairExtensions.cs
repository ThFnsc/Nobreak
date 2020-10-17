using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class KeyValuePairExtensions
    {
        public static string ToLines(this IEnumerable<KeyValuePair<object, object>> input) =>
            string.Join('\n', input.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }
}
