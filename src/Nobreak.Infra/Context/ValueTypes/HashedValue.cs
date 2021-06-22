using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Linq;

namespace System.Security.Cryptography
{
    public readonly struct HashedValue
    {
        private readonly KeyDerivationPrf _algorithm;
        private readonly byte[] _salt;
        private readonly byte[] _hash;

        private HashedValue(byte[] salt, byte[] hash, KeyDerivationPrf algorithm)
        {
            _salt = salt;
            _hash = hash;
            _algorithm = algorithm;
        }

        public static HashedValue ParseFromHash(string hash) =>
            TryParseFromHash(hash) ?? throw new FormatException();

        public static HashedValue? TryParseFromHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return null;
            var parts = hash.Split(':');
            if (parts.Length != 3)
                return null;
            if (!Enum.TryParse(parts[0], out KeyDerivationPrf algorithm))
                return null;
            var base64 = parts.Skip(1).Select(p =>
            {
                try
                {
                    return Convert.FromBase64String(p);
                }
                catch
                {
                    return null;
                }
            }).ToArray();
            if (base64.Any(b => b == null))
                return null;
            return new HashedValue(base64[0], base64[1], algorithm);
        }

        private static byte[] RandomBytes(int bytes)
        {
            using var rng = RandomNumberGenerator.Create();
            var res = new byte[bytes];
            rng.GetBytes(res);
            return res;
        }

        public static HashedValue FromValue(string value) =>
            FromValue(value, 32, KeyDerivationPrf.HMACSHA256);

        public static HashedValue FromValue(string value, int saltLength, KeyDerivationPrf algorithm)
        {
            if (value == null)
                throw new ArgumentException(nameof(value));
            var salt = RandomBytes(saltLength);
            var hash = HashValue(value, salt, algorithm);
            return new HashedValue(salt, hash, algorithm);
        }

        private static byte[] HashValue(string value, byte[] salt, KeyDerivationPrf algorithm, int bytes = 64) =>
            KeyDerivation.Pbkdf2(value, salt, algorithm, 10000, bytes);

        public bool Check(string value)
        {
            var newHash = HashValue(value, _salt, _algorithm, _hash.Length);
            var equal = newHash.SequenceEqual(_hash);
            return equal;
        }

        public override bool Equals(object obj)
        {
            if (obj is string value)
                return Check(value);
            if (obj is HashedValue hashedValue)
                return hashedValue._hash.SequenceEqual(_hash) && hashedValue._algorithm == _algorithm && hashedValue._salt.SequenceEqual(_salt);
            return false;
        }

        public static implicit operator HashedValue(string valueToHash) => FromValue(valueToHash);
        public static implicit operator string(HashedValue hashedValue) => hashedValue.ToString();

        public static bool operator ==(HashedValue hashedValue, string value) => hashedValue.Equals(value);
        public static bool operator !=(HashedValue hashedValue, string value) => !hashedValue.Equals(value);

        public static bool operator ==(string value, HashedValue hashedValue) => hashedValue.Equals(value);
        public static bool operator !=(string value, HashedValue hashedValue) => !hashedValue.Equals(value);

        public override string ToString() =>
            $"{_algorithm}:{Convert.ToBase64String(_salt)}:{Convert.ToBase64String(_hash)}";

        public override int GetHashCode() =>
            HashCode.Combine(_algorithm, _salt, _hash);
    }
}
