using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Template.Api.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configure();

    builder.Services.Configure(builder.Configuration);

    var app = builder.Build();

    app.UseMiddlewares(app.Environment, app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

    app.Run();
}

catch (Exception ex)
{
    Console.WriteLine(ex);
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

//This is to ensure we can extend this class in integration testsxwxs
public partial class Program { }
