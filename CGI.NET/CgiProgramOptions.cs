using Common;

namespace Osprey
{
    public class CgiProgramOptions
    {
        public HttpMethod Method { get; set; }
        public string Route { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
    }
}
