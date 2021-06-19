using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class IListExtensions
    {
        public static T RelativeTo<T>(this IList<T> list, T target, int offset) =>
            list[list.Contains(target)
                ? (list.IndexOf(target) + offset).Mod(list.Count)
                : throw new ArgumentException("Alvo não existe")];

        public static T Before<T>(this IList<T> list, T target) =>
            list.RelativeTo(target, -1);

        public static T After<T>(this IList<T> list, T target) =>
            list.RelativeTo(target, 1);

        public static IList<T> AddRange<T>(this IList<T> list, params T[] items) =>
            list.AddRange(items.AsEnumerable());

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            items.ForEach(list.Add);
            return list;
        }
    }
}