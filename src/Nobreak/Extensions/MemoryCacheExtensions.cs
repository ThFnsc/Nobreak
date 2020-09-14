using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Memory
{
    public static class MemoryCacheExtensions
    {
        public static Task<T> GetAsync<T>(this IMemoryCache memoryCache, object identifier, Func<Task<T>> getValue, TimeSpan ttl) =>
            memoryCache.TryGetValue(identifier, out object cached)
                ? (Task<T>)cached
                : memoryCache.Set(identifier, Task.Run(getValue), ttl);
    }
}
