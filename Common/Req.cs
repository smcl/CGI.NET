using System.Collections.Generic;

namespace Common
{
    public class Req
    {
        public string Path { get; set; }
        public HttpMethod Method { get; set; }
        public IEnumerable<(string, string)> Headers { get; set; }
        public IDictionary<string, string> Query { get; set; }
        public IEnumerable<char> Body { get; set; }
    }
}