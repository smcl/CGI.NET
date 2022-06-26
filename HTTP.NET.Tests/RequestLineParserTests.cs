using Common;
using Osprey.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;

namespace Osprey.Tests
{
    [TestClass]
    public class RequestLineParserTests
    {
        [TestMethod]
        public void TestSimpleGetLine()
        {
            const string TestRequestLine = "GET /home.html HTTP/1.1\r\n";
            var (method, path, queryString, version) = RequestLineGrammar.Parser.Parse(TestRequestLine);

            Assert.AreEqual(HttpMethod.Get, method);
            Assert.AreEqual("/home.html", path);
            Assert.AreEqual(HttpVersion.Version1_1, version);
        }

        [TestMethod]
        public void TestSimplePostLine()
        {
            const string TestRequestLine = "POST /api/v2/some/resource HTTP/1.1\r\n";
            var (method, path, queryString, version) = RequestLineGrammar.Parser.Parse(TestRequestLine);

            Assert.AreEqual(HttpMethod.Post, method);
            Assert.AreEqual("/api/v2/some/resource", path);
            Assert.AreEqual(HttpVersion.Version1_1, version);
        }

        [TestMethod]
        public void TestSimplePutLine()
        {
            const string TestRequestLine = "PUT /api/v2/some/resource HTTP/1.1\r\n";
            var (method, path, queryString, version) = RequestLineGrammar.Parser.Parse(TestRequestLine);

            Assert.AreEqual(HttpMethod.Put, method);
            Assert.AreEqual("/api/v2/some/resource", path);
            Assert.AreEqual(HttpVersion.Version1_1, version);
        }

        [TestMethod]
        public void TestSimpleDeleteLine()
        {
            const string TestRequestLine = "DELETE /api/v2/some/resource HTTP/1.1\r\n";
            var (method, path, queryString, version) = RequestLineGrammar.Parser.Parse(TestRequestLine);

            Assert.AreEqual(HttpMethod.Delete, method);
            Assert.AreEqual("/api/v2/some/resource", path);
            Assert.AreEqual(HttpVersion.Version1_1, version);
        }

        [TestMethod]
        public void TestGetWithQueryString()
        {
            const string TestRequestLine = "GET /api/v2/greet?name=sean HTTP/1.1\r\n";
            var (method, path, queryString, version) = RequestLineGrammar.Parser.Parse(TestRequestLine);

            Assert.AreEqual(HttpMethod.Get, method);
            Assert.AreEqual("/api/v2/greet", path);
            Assert.AreEqual("sean", queryString["name"]);
            Assert.AreEqual(HttpVersion.Version1_1, version);
        }
    }
}