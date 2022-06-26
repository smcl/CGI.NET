using System.Collections.Generic;

namespace Osprey.Routing
{
    public class RootRouteNode : RouteNode
    {
        public RootRouteNode(): base()
        {
        }

        public override RouteResult? TryMatch(string currentRoutePart, IEnumerable<string> remainingRouteParts, IEnumerable<(string key, string value)> routeValues)
        {
            foreach (var child in Children)
            {
                var result = child.TryMatch(currentRoutePart, remainingRouteParts, routeValues);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
