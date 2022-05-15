
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;
using System.Linq;

namespace CGI.NET
{
    public class Runner
    {
        public static string Execute(HttpRequest request, string command, string args)
        {
            var process = new Process();
            // var startInfo = new ProcessStartInfo();
            process.StartInfo.UseShellExecute = false; //required to redirect standard input/output

            // redirects on your choice
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            var syncIOFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            // set headers to environment variables
            foreach (var header in request.Headers)
            {
                process.StartInfo.EnvironmentVariables.Add(header.Key, header.Value);
            }

            // write query string to QUERY_STRING
            var routeValues = request.RouteValues.Select(rv => (rv.Key, rv.Value.ToString()));
            var queryStringValues = request.Query.Select(qv => (qv.Key, qv.Value.ToString()));

            var allValues = routeValues.Union(queryStringValues).Select(kvp => $"{kvp.Key}={kvp.Item2}");
            if (allValues.Any())
            {
                var queryString = string.Join("&", allValues);
                process.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
            }

            process.Start();

            using (var reader = new StreamReader(request.Body))
            {
                const int chunkLength = 1024;
                var readChunk = new char[chunkLength];
                int readChunkLength;
                do
                {
                    readChunkLength = reader.Read(readChunk, 0, chunkLength);
                    process.StandardInput.Write(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);
            }

            process.WaitForExit();

            Console.WriteLine(process.StandardError.ReadToEnd());

            return process.StandardOutput.ReadToEnd();
        }
    }
}
