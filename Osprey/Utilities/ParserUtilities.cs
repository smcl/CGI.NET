using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osprey.Utilities
{
    internal class ParserUtilities
    {
        public static Parser<T> ParseAny<T>(params Parser<T>[] parsers) => parsers.Aggregate((a, b) => a.Or(b));
    }
}
