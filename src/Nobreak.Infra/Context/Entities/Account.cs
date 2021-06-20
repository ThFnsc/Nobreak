using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Nobreak.Context.Entities
{
    [Index(nameof(Email))]
    public class Account : Entity
    {
        [Required]
        public string Name { get; private set; }

        [Required]
        public string Email { get; private set; }

        public HashedValue PasswordHash { get; private set; }

        internal Account() { }

        public Account(string name, string email, string password)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email;
            if (password?.Length < 8)
                throw new ArgumentException(nameof(password));
            PasswordHash = password;
        }

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
