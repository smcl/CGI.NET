using Common;
using Osprey.Utilities;
using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace Osprey.Parsing
{
    public class RequestLineGrammar
    {
        private static Parser<HttpMethod> ParseMethod = ParserUtilities.ParseAny(
                Parse.IgnoreCase("GET").Return(HttpMethod.Get),
                Parse.IgnoreCase("DELETE").Return(HttpMethod.Delete),
                Parse.IgnoreCase("POST").Return(HttpMethod.Post),
                Parse.IgnoreCase("PUT").Return(HttpMethod.Put),
                Parse.IgnoreCase("PATCH").Return(HttpMethod.Patch),
                Parse.IgnoreCase("OPTIONS").Return(HttpMethod.Patch)
            );

        private static Parser<HttpVersion> ParseVersion =
            ParserUtilities.ParseAny(
                Parse.String("HTTP/1.1").Return(HttpVersion.Version1_1),
                Parse.String("HTTP/2").Return(HttpVersion.Version2)
            );


        private static Parser<(string key, string value)> ParseKeyValue =
            from key in Parse.AnyChar.Except(Parse.WhiteSpace.Or(Parse.Char('='))).Many().Text()
            from _ in Parse.Char('=')
            from value in Parse.AnyChar.Except(Parse.WhiteSpace.Or(Parse.Char('='))).Many().Text()
            select (key, value);


        private static Parser<IDictionary<string, string>> ParseQueryString =
            from _ in Parse.Char('?')
            from queryValues in ParseKeyValue.Many()
            select queryValues.ToDictionary(qv => qv.key, qv => qv.value);


        public static Parser<(HttpMethod method, string path, IDictionary<string, string> queryString, HttpVersion version)> Parser =
            from method in ParseMethod
            from _1 in Parse.WhiteSpace
            from path in Parse.AnyChar.Except(Parse.WhiteSpace.Or(Parse.Char('?'))).Many().Text()
            from queryString in ParseQueryString.Optional()
            from _2 in Parse.WhiteSpace
            from version in ParseVersion
            from _3 in Common.CrLf
            select (method, path, queryString.GetOrDefault(), version);
    }
}
