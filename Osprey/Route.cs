using Common;
using System;

namespace Osprey
{
    public class Route
    {
        public string Method { get; set; }
        public string Pattern { get; set; }
        public Func<Req, string> Handler { get; set; }
    }
}
