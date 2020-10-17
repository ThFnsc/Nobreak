using Nobreak.Context.Entities;
using Nobreak.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Nobreak.Entities
{
    public class NobreakContext : DbContext
    {
        public DbSet<NobreakState> NobreakStates { get; set; }

        public DbSet<NobreakStateChange> NobreakStateChanges { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public NobreakContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.PasswordHash)
                .HasConversion(p => p.ToString(), p => HashedValue.ParseFromHash(p));
        }
    }
}