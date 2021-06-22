using Nobreak.Context.Entities;
using Nobreak.Infra.Context.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Infra.Context
{
    public interface IDbContext : IDisposable
    {
        IQueryable<NobreakState> NobreakStates { get; }

        IQueryable<NobreakStateChange> NobreakStateChanges { get; }

        IQueryable<Account> Accounts { get; }

        Task SaveChangesAsync();

        T Add<T>(T entity) where T : Entity;

        IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity;

        void Migrate();
    }
}
