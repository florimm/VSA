using Microsoft.AspNetCore.Mvc.ApplicationModels;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Template.Api.Common.Health.DaprServiceHealth;
using Template.Api.Common.Pipelines;

namespace Template.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"], "template", "template", configuration["TEMPLATE_VERSION"], configuration["ASPNETCORE_ENVIRONMENT"]);
        services.AddServiceHttpClients(configuration);
        services.RegisterServicesByFeature();
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddTemplateHealthChecks();
        services.AddDbContext<DatabaseContext>((s, o) =>
            o.UseNpgsql(configuration["Database:ConnectionString"]));

        services.AddScoped<IDatabaseContext, DatabaseContext>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });
        services.AddSingleton(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddHttpContextAccessor();
        services.AddControllersWithViews(options => options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()))).AddDapr();
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, string? otelEndpointOrNull, string serviceName, string nameSpace, string? version, string? environment)
    {
        if (!string.IsNullOrWhiteSpace(otelEndpointOrNull))
        {
            services.AddOpenTelemetry()
                .ConfigureResource(r =>
                {
                    r.AddService(serviceName, nameSpace, version, false, Environment.MachineName);
                    r.AddAttributes(new Dictionary<string, object>
                    {
                        {"deployment.environment", environment ?? "unset" }
                    });
                })
                .WithTracing(builder => builder
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddHttpClientInstrumentation(options => options.RecordException = true)
                    .AddOtlpExporter(options => options.Endpoint = new Uri(otelEndpointOrNull)))
                .WithMetrics(builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options => options.Endpoint = new Uri(otelEndpointOrNull)));
        }
        return services;
    }

    public static IServiceCollection AddServiceHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");

        AddDaprClient("Auth", "auth.auth");
        AddDaprClient("User", "user.user");
        AddDaprClient("Onboarding", "onboarding.onboarding");

        void AddHttpClient(string client, string? baseUrl = null)
        {
            var baseAddress = baseUrl ?? configuration[$"{client}:BaseUrl"] ?? throw new ConfigurationException($"could not configure http client {client}. Base address was null");
            //Daprized address for non-dapr recipients.
            services.AddHttpClient(client, _ => _.BaseAddress = new Uri($"http://localhost:{daprPort}/v1.0/invoke/{baseAddress}method/"));
        }

        void AddDaprClient(string client, string appId) => services.AddHttpClient(client,
                c =>
                {
                    c.BaseAddress = new Uri($"http://localhost:{daprPort}/");
                    c.DefaultRequestHeaders.Add("dapr-app-id", appId);
                });

        return services;
    }

    public static void RegisterServicesByFeature(this IServiceCollection services)
    {
        {
            var className = "StartupExtensions";
            var methodName = "Configure";
            var assembly = Assembly.GetExecutingAssembly();
            var classTypes = assembly.GetTypes()
            .Where(t => t.Name == className)
            .ToArray();
            foreach (var type in classTypes)
            {
                Console.WriteLine($"registering service: {type.Namespace}");
                var configureMethod = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
                configureMethod?.Invoke(null, new object[] { services });
            }
        }
    }

    public static IServiceCollection AddTemplateHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
                .AddTypeActivatedCheck<GenericDaprServiceHealth>("Auth", null, new[] { "ready" }, "Auth", true)
                .AddTypeActivatedCheck<GenericDaprServiceHealth>("Onboarding", null, new[] { "ready" }, "Onboarding", true)
                .AddTypeActivatedCheck<GenericDaprServiceHealth>("User", null, new[] { "ready" }, "User", true)

                .AddTypeActivatedCheck<GenericDaprServiceHealth>("Startup Auth", null, new[] { "startup" }, "Auth", false)
                .AddTypeActivatedCheck<GenericDaprServiceHealth>("Startup Onboarding", null, new[] { "startup" }, "Onboarding", false)
                .AddTypeActivatedCheck<GenericDaprServiceHealth>("Startup User", null, new[] { "startup" }, "User", false)
                ;

        return services;
    }
}
