using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
