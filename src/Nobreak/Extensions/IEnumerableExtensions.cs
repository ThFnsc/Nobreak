using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class IEnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> collection) where T : class =>
            collection is IList<T> asList
                ? asList.Count == 0
                    ? null
                    : asList[new Random().Next(0, asList.Count)]
                : collection.ToList().Random();

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> input) =>
            input.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static bool Contains<T>(this IEnumerable<T> list, Predicate<T> predicate)
        {
            foreach (var element in list)
                if (predicate(element))
                    return true;
            return false;
        }
            
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var element in list)
                action(element);
        }
    }
}
