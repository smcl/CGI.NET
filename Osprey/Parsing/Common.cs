using Sprache;

namespace Osprey.Parsing
{
    public class Common
    {
        public static Parser<string> CrLf = Parse.String("\r\n").Text();
    }
}
