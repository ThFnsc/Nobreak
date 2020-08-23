using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
