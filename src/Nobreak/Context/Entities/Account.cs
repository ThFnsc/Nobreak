using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nobreak.Context.Entities
{
    public class Account : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public IEnumerable<Claim> Claims() =>
            new List<Claim>
            {
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim("FirstName", Name.FirstWord())
            };
    }
}
