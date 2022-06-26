using Osprey.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.Linq;

namespace Osprey.Tests
{
    [TestClass]
    public class RequestHeaderTests
    {
        [TestMethod]
        public void TestSingleHeader()
        {
            const string TestHeader = "x-forwarded-for: 192.168.0.1\r\n";
            var (name, value) = RequestHeaderGrammar.Parser.Parse(TestHeader);

            Assert.AreEqual(name, "x-forwarded-for");
            Assert.AreEqual(value, "192.168.0.1");
        }

        [TestMethod]
        public void TestMultipleHeaders()
        {
            const string TestHeader = @"x-forwarded-for: 192.168.0.1
x-foo: bar
";
            var headersParser = RequestHeaderGrammar.Parser.Many();
            var headers = headersParser.Parse(TestHeader).ToList();

            foreach (var (name, value) in headers)
            {
                if (name == "x-foo")
                {
                    Assert.AreEqual("bar", value);
                }
            }
        }
    }
}