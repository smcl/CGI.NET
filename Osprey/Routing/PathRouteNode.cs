using Osprey.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Osprey.Routing
{
    public class PathRouteNode : RouteNode
    {
        private readonly string _name;

        public PathRouteNode(string name)
        {
            _name = name;
        }

        public override RouteResult? TryMatch(string currentRoutePart, IEnumerable<string> remainingRouteParts, IEnumerable<(string key, string value)> routeValues)
        {
            if (_name.ToLower() == currentRoutePart.ToLower())
            {
                if (!remainingRouteParts.Any() && !Children.Any())
                {
                    return new RouteResult
                    {
                        Handler = Handler!,
                        RouteValues = routeValues.ToDictionary(rv => rv.key, rv => rv.value)
                    };
                }

                foreach (var child in Children)
                {
                    var result = child.TryMatch(remainingRouteParts.Car(), remainingRouteParts.Cdr(), routeValues);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}
