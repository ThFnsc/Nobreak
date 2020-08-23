using Nobreak.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nobreak.Services
{
    public class CachedResources : MemoryCache, IDisposable
    {
        private readonly MemoryCache _memoryCache;
        
        private readonly Dictionary<object, Tuple<Func<Task<object>>, TimeSpan>> _factories;

        public CachedResources():base(new MemoryCacheOptions())
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _factories = new Dictionary<object, Tuple<Func<Task<object>>, TimeSpan>>();
        }

        public void Register<T>(Func<Task<T>> factory, TimeSpan ttl, object identifier) =>
            _factories[identifier] = new Tuple<Func<Task<object>>, TimeSpan>(async () => await factory(), ttl);

        public void Register<T>(Func<Task<T>> factory, TimeSpan ttl) =>
            Register(factory, ttl, typeof(T));

        public void RegisterWithDisposibleService<T, TService>(Func<TService, Task<T>> factory, IServiceProvider serviceProvider, TimeSpan ttl, object identifier) where TService : IDisposable =>
            Register(async () =>
            {
                using var scope = serviceProvider.CreateScope();
                using var service = scope.ServiceProvider.GetService<TService>();
                return await factory(service);
            }, ttl, identifier);

        public void RegisterWithDisposibleService<T, TService>(Func<TService, Task<T>> factory, IServiceProvider serviceProvider, TimeSpan ttl) where TService : IDisposable =>
            RegisterWithDisposibleService(factory, serviceProvider, ttl, typeof(T));

        public async Task<T> GetAsync<T>(object identifier) =>
            await _memoryCache.GetOrCreateAsync<T>(identifier, async entry =>
            {
                if (_factories.TryGetValue(identifier, out var a)) {
                    entry.AbsoluteExpirationRelativeToNow = a.Item2;
                    return (T)await a.Item1();
                }
                throw new ArgumentException("Identificador não foi registrado previamente");
            });

        public async Task<T> GetAsync<T>() =>
            await GetAsync<T>(typeof(T));

        public void Invalidate(object identifier) =>
            _memoryCache.Remove(identifier);

        public void Invalidate<T>() =>
            Invalidate(typeof(T));
    }
}
