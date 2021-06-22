using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class IEnumerableExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> input) =>
            input.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
