using System.Collections.Generic;

namespace Osprey
{
    public class Binding
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public IEnumerable<CgiProgramOptions> Routes { get; set; }
    }
}
