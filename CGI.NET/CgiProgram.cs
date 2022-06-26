using Common;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;

namespace Osprey
{
    public class CgiProgram
    {
        public static string HandleRequestAsync(Req request, string command, string args)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            WriteHeadersToEnvironment(request, process);
            WriteQueryString(request, process);

            process.Start();
            WriteRequestBodyToStdin(request, process);

            process.WaitForExit();

            System.Console.WriteLine(process.StandardError.ReadToEnd());

            return process.StandardOutput.ReadToEnd();
        }

        private static void WriteHeadersToEnvironment(Req request, Process process)
        {
            foreach (var (key, value) in request.Headers)
            {
                process.StartInfo.EnvironmentVariables.Add(key, value);
            }
        }

        private static void WriteQueryString(Req request, Process process)
        {
            if (request.Query != null && request.Query.Any())
            {
                var queryValues = request.Query.Select(q => $"{q.Key}={q.Value}");
                var queryString = string.Join("&", queryValues);
                process.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
            }
        }

        private static void WriteRequestBodyToStdin(Req request, Process process)
        {
            process.StandardInput.Write(request.Body);
            // TODO: should really be dealing with streams here, not string
            //await request.BodyReader.CopyToAsync(pw, cancellationToken);
        }
    }
}
