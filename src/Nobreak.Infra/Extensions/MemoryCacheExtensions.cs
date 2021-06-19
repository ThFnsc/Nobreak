using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Memory
{
    public static class MemoryCacheExtensions
    {
        public static Task<T> GetAsync<T>(this IMemoryCache memoryCache, object identifier, Func<Task<T>> getValue, TimeSpan ttl) =>
            memoryCache.TryGetValue(identifier, out var cached)
                ? (Task<T>) cached
                : memoryCache.Set(identifier, Task.Run(getValue), ttl);

        public static Task<TOutput> GetWithServiceAsync<TOutput, TService>(this IMemoryCache memoryCache, IServiceProvider serviceProvider, object identifier, Func<TService, Task<TOutput>> getValue, TimeSpan ttl) =>
            memoryCache.GetAsync(identifier, async () =>
            {
                using var scope = serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                return await getValue(service);
            }, ttl);
    }
}
