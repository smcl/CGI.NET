using Common;
using Osprey.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Osprey.Tests
{
    [TestClass]
    public class RouterTests
    {
        [TestMethod]
        public void TestSimple()
        {
            var router = new Router();

            var fooExecuted = false;
            var barExecuted = false;

            var fooAction = (Req request) => { fooExecuted = true; return string.Empty; };
            var barAction = (Req request) => { barExecuted = true; return string.Empty; };

            router.AddRoute(HttpMethod.Get, "api/foo", fooAction);
            router.AddRoute(HttpMethod.Get, "api/bar", barAction);

            var result = router.TryMatch(HttpMethod.Get, "api/foo");

            Assert.IsNotNull(result);
            result.Handler(new Req());

            Assert.IsTrue(fooExecuted);
            Assert.IsFalse(barExecuted);
        }

        [TestMethod]
        public void TestSimpleRouteValue()
        {
            var router = new Router();

            var helloExecuted = false;
            var goodbyeExecuted = false;

            var helloAction = (Req request) => { helloExecuted = true; return string.Empty; };
            var barAction = (Req request) => { goodbyeExecuted = true; return string.Empty; };

            router.AddRoute(HttpMethod.Get, "api/hello/{name}", helloAction);
            router.AddRoute(HttpMethod.Get, "api/goodbye/{name}", barAction);

            var result = router.TryMatch(HttpMethod.Get, "api/hello/sean");

            Assert.IsNotNull(result);
            result.Handler(new Req());

            Assert.IsTrue(helloExecuted);
            Assert.IsFalse(goodbyeExecuted);

            Assert.AreEqual(result.RouteValues["name"], "sean");
        }

        [TestMethod]
        public void TestEmptyRoutes()
        {
            var router = new Router();

            var result = router.TryMatch(HttpMethod.Get, "api/hello/sean");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNoMatchingRoutes()
        {
            var router = new Router();

            var fooAction = (Req request) => { return string.Empty; };
            var barAction = (Req request) => { return string.Empty; };

            router.AddRoute(HttpMethod.Get, "api/hello/{name}", fooAction);
            router.AddRoute(HttpMethod.Get, "api/goodbye/{name}", barAction);

            var result = router.TryMatch(HttpMethod.Get, "api/smello/sean");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestCorrectMethod()
        {
            var router = new Router();

            var getExecuted = false;
            var postExecuted = false;

            var getAction = (Req request) => { getExecuted = true; return string.Empty; };
            var postAction = (Req request) => { postExecuted = true; return string.Empty; };

            router.AddRoute(HttpMethod.Get, "api", getAction);
            router.AddRoute(HttpMethod.Post, "api", postAction);

            var getResult = router.TryMatch(HttpMethod.Get, "api");
            var postResult = router.TryMatch(HttpMethod.Post, "api");

            Assert.IsNotNull(getResult);
            Assert.IsNotNull(postResult);

            getResult.Handler(default);
            Assert.IsTrue(getExecuted);
            Assert.IsFalse(postExecuted);

            postResult.Handler(default);
            Assert.IsTrue(postExecuted);
        }
    }
}
