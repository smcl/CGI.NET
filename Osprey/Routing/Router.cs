using Common;
using Osprey.Utilities;
using System;
using System.Collections.Generic;

namespace Osprey.Routing
{
    public class Router
    {
        private IDictionary<HttpMethod, RouteNode> _methods;

        public Router()
        {
            _methods = new Dictionary<HttpMethod, RouteNode>();
        }

        public void AddRoute(HttpMethod method, string route, Func<Req, string> handler)
        {
            var routeParts = route.Split("/");

            if (!_methods.TryGetValue(method, out var root))
            {
                root = new RootRouteNode();
                _methods.Add(method, root);
            }

            root.AddChild(routeParts.Car(), routeParts.Cdr(), handler);
        }

        public RouteResult? TryMatch(HttpMethod method, string route)
        {
            if (_methods.TryGetValue(method, out var root))
            {
                var routeParts = route.Split("/");
                return root.TryMatch(routeParts.Car(), routeParts.Cdr(), new List<(string, string)>());
            }

            return null;
        }
    }
}
