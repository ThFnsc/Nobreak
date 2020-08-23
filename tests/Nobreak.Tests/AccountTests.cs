using Nobreak.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Nobreak.Tests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void PasswordTests()
        {
            var passwords = Enumerable.Range(1, 10)
                .Select(i => Helpers.RandomBase64(64))
                .Distinct()
                .Select(p => new KeyValuePair<string, string>(p, PasswordHasher.Hash(p)))
                .ToList();
            passwords.ForEach(p =>
                Assert.AreEqual(p, passwords.Single(c => PasswordHasher.Check(p.Key, c.Value))));
        }
    }
}
