using Nobreak.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System;

namespace Nobreak.Tests
{
    [TestClass]
    public class HashedValueTests
    {
        private readonly Random _random;

        public HashedValueTests()
        {
            _random = new Random();
        }

        private string RandomString() =>
            Helpers.RandomBase64(_random.Next(10, 32));

        [TestMethod]
        public void ShouldPredictEqualitiesCorrectly()
        {
            var randomStrings = Enumerable.Range(1, 5)
                .Select(i => RandomString())
                .Distinct()
                .ToList();

            foreach (var password1 in randomStrings)
                foreach (var password2 in randomStrings)
                    Assert.IsTrue((HashedValue)password1 == password2 == (password1 == password2));
        }

        public void ShouldBeSerializableAndParseable()
        {
            var value = RandomString();
            HashedValue hashed = value;
            var copy = HashedValue.ParseFromHash(hashed.ToString());
            Assert.AreEqual(hashed, copy);
            Assert.AreEqual(value, copy);
        }

        public void ShouldBeVersatile()
        {
            var value = RandomString();
            var differentValue = value + "-";
            HashedValue hashed = value;
            Assert.AreEqual(value, hashed);
            Assert.AreNotEqual(differentValue, hashed);

            Assert.IsTrue(value == hashed);
            Assert.IsTrue(hashed == value);
            Assert.IsFalse(hashed != value);
            Assert.IsFalse(value != hashed);

            Assert.IsFalse(differentValue == hashed);
            Assert.IsFalse(hashed == differentValue);
            Assert.IsTrue(hashed != differentValue);
            Assert.IsTrue(differentValue != hashed);
        }
    }
}
