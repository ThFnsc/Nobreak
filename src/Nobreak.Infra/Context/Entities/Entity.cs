using Microsoft.EntityFrameworkCore;
using System;

namespace Nobreak.Context.Entities
{
    [Index(nameof(Timestamp))]
    public abstract class Entity
    {
        public long Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Entity()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
