using Osprey.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Osprey.Routing
{
    public class ParamRouteNode : RouteNode
    {
        private readonly string _name;

        public ParamRouteNode(string name): base()
        {
            _name = name;
        }

        public override RouteResult? TryMatch(string currentRoutePart, IEnumerable<string> remainingRouteParts, IEnumerable<(string key, string value)> routeValues)
        {
            // if they've requested /api/foo/bar and this route is /api/foo, bail
            if (remainingRouteParts.Any() != Children.Any())
            {
                return null;
            }

            var nextRouteValues = routeValues.Concat(new[] { (_name, currentRoutePart) });

            if (!Children.Any())
            {
                return new RouteResult
                {
                    Handler = Handler!,
                    RouteValues = nextRouteValues.ToDictionary(rv => rv.Item1, rv => rv.Item2)
                };
            }

            foreach (var child in Children)
            {
                var result = child.TryMatch(remainingRouteParts.Car(), remainingRouteParts.Cdr(), nextRouteValues);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
