using CGI.NET;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

var programs = app.Configuration.GetSection("CgiBin").Get<IEnumerable<CgiProgram>>();
foreach (var program in programs)
{
    app.MapMethods(program.Route, new[] { program.Method.ToUpper() }, (HttpRequest request) => Runner.Execute(request, program.Command, program.Arguments));
}

app.Run();
