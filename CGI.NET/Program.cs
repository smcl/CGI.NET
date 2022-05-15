using CGI.NET;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;
using System.IO.Pipelines;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

var programs = app.Configuration.GetSection("CgiBin").Get<IEnumerable<CgiProgram>>();
foreach (var program in programs)
{
    app.MapMethods(program.Route, new[] { program.Method.ToUpper() }, (HttpRequest request) => HandleRequestAsync(request, program.Command, program.Arguments));
}

app.Run();

async Task<string> HandleRequestAsync(HttpRequest request, string command, string args)
{
    var cancellationToken = request.HttpContext.RequestAborted;
    var process = new Process();
    process.StartInfo.UseShellExecute = false; //required to redirect standard input/output

    // redirects on your choice
    process.StartInfo.RedirectStandardInput = true;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;

    process.StartInfo.CreateNoWindow = true;

    process.StartInfo.FileName = command;
    process.StartInfo.Arguments = args;
    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

    var bodyControl = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
    if (bodyControl != null)
    {
        bodyControl.AllowSynchronousIO = true;
    }

    WriteHeadersToEnvironment(request, process);
    WriteQueryString(request, process);

    process.Start();
    await WriteRequestBodyToStdinAsync(request, process, cancellationToken);

    process.WaitForExit();

    Console.WriteLine(process.StandardError.ReadToEnd());

    return process.StandardOutput.ReadToEnd();
}

void WriteHeadersToEnvironment(HttpRequest request, Process process)
{
    foreach (var header in request.Headers)
    {
        process.StartInfo.EnvironmentVariables.Add(header.Key, header.Value);
    }
}

void WriteQueryString(HttpRequest request, Process process)
{
    // write query string to QUERY_STRING
    var routeValues = request.RouteValues.Select(rv => (rv.Key, rv.Value.ToString()));
    var queryStringValues = request.Query.Select(qv => (qv.Key, qv.Value.ToString()));

    var allValues = routeValues.Union(queryStringValues).Select(kvp => $"{kvp.Key}={kvp.Item2}");
    if (allValues.Any())
    {
        var queryString = string.Join("&", allValues);
        process.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
    }
}

async Task WriteRequestBodyToStdinAsync(HttpRequest request, Process process, CancellationToken cancellationToken)
{
    var pw = PipeWriter.Create(process.StandardInput.BaseStream);
    await request.BodyReader.CopyToAsync(pw, cancellationToken);
}