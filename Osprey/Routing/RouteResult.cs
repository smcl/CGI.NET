using Common;
using System;
using System.Collections.Generic;

namespace Osprey.Routing
{
    public class RouteResult
    {
        public Func<Req, string> Handler { get; set; }
        public IDictionary<string, string> RouteValues { get; set; }
    }
}
