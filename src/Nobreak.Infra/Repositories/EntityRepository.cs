using Nobreak.Context.Entities;
using System.Linq;

namespace Nobreak.Infra.Repositories
{
    public static class EntityRepository
    {
        public static IQueryable<T> OfId<T>(this IQueryable<T> input, int id) where T : Entity =>
            input.Where(e => e.Id == id);
    }
}
