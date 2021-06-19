using Microsoft.EntityFrameworkCore;
using Nobreak.Context.Entities;
using Nobreak.Infra.Context.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Nobreak.Infra.Context
{
    public class DataContect : DbContext, IDbContext
    {
        public DbSet<NobreakState> NobreakStates { get; set; }

        public DbSet<NobreakStateChange> NobreakStateChanges { get; set; }

        public DbSet<Account> Accounts { get; set; }

        IQueryable<NobreakState> IDbContext.NobreakStates => NobreakStates.AsQueryable();

        IQueryable<NobreakStateChange> IDbContext.NobreakStateChanges => NobreakStateChanges.AsQueryable();

        IQueryable<Account> IDbContext.Accounts => Accounts.AsQueryable();

        public DataContect(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.PasswordHash)
                .HasConversion(p => p.ToString(), p => HashedValue.ParseFromHash(p));

            modelBuilder.Entity<Account>()
                .Property(a => a.Email)
                .HasConversion(p => p.ToString(), p => p);

            modelBuilder.Entity<NobreakState>()
                .HasIndex(s => s.Timestamp)
                .IsUnique();
        }

        public Task SaveChangesAsync() =>
            base.SaveChangesAsync();

        T IDbContext.Add<T>(T entity)
        {
            base.Add(entity);
            return entity;
        }

        public IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity
        {
            base.AddRange(entities);
            return entities;
        }
    }
}