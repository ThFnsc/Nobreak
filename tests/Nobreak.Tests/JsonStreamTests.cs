using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Nobreak.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nobreak.Tests
{
    [TestClass]
    public class JsonStreamTests
    {
        [TestMethod]
        public void JsonShouldBeEqual()
        {
            var objectArray = Enumerable.Range(0, 30).Select(i => new { test = Helpers.RandomBase64(10000), test2 = Helpers.RandomBase64(10000) }).ToList();
            using var jsonStream = new JsonStream(objectArray);
            using var streamReader = new StreamReader(jsonStream);
            var streamText = streamReader.ReadToEnd();
            var jsonText = JsonConvert.SerializeObject(objectArray, Formatting.None);
            Assert.AreEqual(streamText, jsonText);
        }
    }
}
