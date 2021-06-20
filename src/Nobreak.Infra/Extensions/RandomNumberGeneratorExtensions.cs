using System;
using System.Security.Cryptography;

namespace Nobreak.Infra.Extensions
{
    public static class RandomNumberGeneratorExtensions
    {
        public static string RandomBase64Url(int length = 128)
        {
            var res = new byte[(int) Math.Ceiling(length * 6.0 / 8.0)];
            RandomNumberGenerator.Fill(res);
            var base64 = Convert.ToBase64String(res).Substring(0, length);
            return base64
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
