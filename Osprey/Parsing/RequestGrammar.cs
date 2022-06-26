using Common;
using Sprache;

namespace Osprey.Parsing
{

    public class RequestGrammar
    {
        public static Parser<Req> Request =>
            from requestLine in RequestLineGrammar.Parser
            from headers in RequestHeaderGrammar.Parser.Many()
            from blankLine in Common.CrLf
            from body in Parse.AnyChar.Many()
            select new Req
            {
                Method = requestLine.method,
                Path = requestLine.path,
                Headers = headers,
                Body = body
            };
    }
}
