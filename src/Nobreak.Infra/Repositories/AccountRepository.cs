using Nobreak.Context.Entities;
using System.Linq;

namespace Nobreak.Infra.Repositories
{
    public static class AccountRepository
    {
        public static IQueryable<Account> OfEmail(this IQueryable<Account> input, string email) =>
            input.Where(e => e.Email.ToLower() == email.ToLower());
    }
}
