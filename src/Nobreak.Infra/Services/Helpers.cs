using System;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Nobreak.Infra.Services
{
    public class Helpers
    {
        public static byte[] RandomBytes(int bytes)
        {
            using var rng = RandomNumberGenerator.Create();
            var res = new byte[bytes];
            rng.GetBytes(res);
            return res;
        }

        public static string RandomBase64(int length) =>
            Convert.ToBase64String(RandomBytes((int) Math.Ceiling(length * 6.0 / 8.0))).Substring(0, length);

        public static string RandomBase64Url(int length) =>
            RandomBase64(length).Replace('+', '-').Replace('/', '_');

        public static string GetPropertyName<T>(Expression<Func<T, object>> lambda)
        {
            if (!(lambda.Body is MemberExpression body))
                body = (lambda.Body as UnaryExpression).Operand as MemberExpression;
            return body.Member.Name;
        }

        public static string GetPropertyNameCamelCase<T>(Expression<Func<T, object>> lambda) =>
            GetPropertyName(lambda).ToCamelCase();
    }
}
