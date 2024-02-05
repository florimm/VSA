using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Template.Api.Common.Health;
using Template.Api.Common.Utils;

namespace Template.Api.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMiddlewares(this WebApplication app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        app.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = LogHelper.ExcludeHealthChecks;
        });
        app.UseMiddleware<FluentValidationExceptionHandlerMiddleware>();

        if (env.IsDevelopment() || env.IsLocal())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = $"Swagger - {Assembly.GetExecutingAssembly().GetName().Name}";
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpper());
                }
            });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        app.UseCloudEvents();
        app.UseAuthorization();
        app.MapControllers();
        app.MapSubscribeHandler();

        app.MapTemplateHealthEndpoints();

        return app;
    }

    public static WebApplication MapTemplateHealthEndpoints(this WebApplication app)
    {
        //For liveness probe we are only concerned with if the service can answer at all, so no more checks performed
        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = healthCheck => false,
            ResponseWriter = ResponseWriter.WriteResponse
        });

        //For readiness probe we must make sure the checks marked with tag: ready all pass.
        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
            ResponseWriter = ResponseWriter.WriteResponse
        });

        //For startup probe we must make sure the checks marked with tag: startup all pass.
        app.MapHealthChecks("/health/startup", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("startup"),
            ResponseWriter = ResponseWriter.WriteResponse
        });

        return app;
    }
}
