using Common;
using Osprey.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.Linq;

namespace Osprey.Tests
{

    [TestClass]
    public class RequestParserTests
    {
        [TestMethod]
        public void TestGetRequest()
        {
            const string TestRequest = @"GET /home.html HTTP/1.1
Host: developer.mozilla.org
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.9; rv:50.0) Gecko/20100101 Firefox/50.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: en-US,en;q=0.5
Accept-Encoding: gzip, deflate, br
Referer: https://developer.mozilla.org/testpage.html
Connection: keep-alive
Upgrade-Insecure-Requests: 1
If-Modified-Since: Mon, 18 Jul 2016 02:36:04 GMT
If-None-Match: ""c561c68d0ba92bbeb8b0fff2a9199f722e3a621a""
Cache-Control: max-age=0

";
            var request = RequestGrammar.Request.Parse(TestRequest);

            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("/home.html", request.Path);

            var connectionHeader = request.Headers.Where(h => h.Item1 == "Connection").Single().Item2;
            Assert.AreEqual("keep-alive", connectionHeader);
        }
    }
}