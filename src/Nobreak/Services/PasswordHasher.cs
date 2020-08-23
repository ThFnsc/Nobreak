using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nobreak.Services
{
    public static class PasswordHasher
    {
        private static string Hash(string passwordPlainText, byte[] salt) =>
            Convert.ToBase64String(KeyDerivation.Pbkdf2(passwordPlainText, salt, KeyDerivationPrf.HMACSHA1, 10000, 32));

        public static string Hash(string passwordPlainText, int saltLength = 16) {
            var salt = Helpers.RandomBytes(saltLength);
            return $"HMACSHA1:{Convert.ToBase64String(salt)}:{Hash(passwordPlainText, salt)}";
        }

        public static bool Check(string passwordPlainText, string knownHash)
        {
            var parts = knownHash.Split(':');
            return Hash(passwordPlainText, Convert.FromBase64String(parts[1])) == parts[2];
        }
    }
}
