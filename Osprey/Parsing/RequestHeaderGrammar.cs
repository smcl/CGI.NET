using Sprache;

namespace Osprey.Parsing
{
    public class RequestHeaderGrammar
    {
        public static Parser<(string name, string value)> Parser =
            from name in Parse.AnyChar.Except(Parse.Char(':').Or(Parse.WhiteSpace)).Many().Text()
            from _1 in Parse.Char(':')
            from _2 in Parse.WhiteSpace.Many()
            from value in Parse.AnyChar.Except(Common.CrLf).Many().Text()
            from _3 in Common.CrLf
            select (name, value);
    }
}
