using System;

namespace Nobreak.Context.Entities
{
    public abstract class Entity
    {
        public long Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Entity()
        {
            Timestamp = DateTime.Now;
        }
    }
}
