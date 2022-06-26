using Common;
using Osprey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osprey.Routing
{
    public abstract class RouteNode
    {
        protected Func<Req, string>? Handler { get; set; }

        protected IList<RouteNode> Children { get; set; }

        public RouteNode()
        {
            Handler = null;
            Children = new List<RouteNode>();
        }

        public abstract RouteResult? TryMatch(string currentRoutePart, IEnumerable<string> remainingRouteParts, IEnumerable<(string key, string value)> routeValues);
        
        public void AddChild(string routePart, IEnumerable<string> remainingRouteParts, Func<Req, string> handler)
        {
            var child = Create(routePart);
            child.Handler = handler; // fuj
            Children.Add(child);

            if (remainingRouteParts.Any())
            {
                child.AddChild(remainingRouteParts.Car(), remainingRouteParts.Cdr(), handler);
            }
        }

        public static RouteNode Create(string currentRoutePart)
        {
            if (currentRoutePart.First() == '{' && currentRoutePart.Last() == '}')
            {
                var withoutBraces = currentRoutePart.Substring(1, currentRoutePart.Length - 2);
                return new ParamRouteNode(withoutBraces);
            }
            else
            {
                return new PathRouteNode(currentRoutePart);
            }
        }
    }
}
