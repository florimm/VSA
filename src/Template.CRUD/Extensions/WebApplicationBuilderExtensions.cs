using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Datadog.Logs;

namespace Template.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();

        if (!(builder.Environment.IsLocal() || string.Equals(builder.Configuration["IntegrationTest"], "true", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AddSecretsManagerConfiguration();
            builder.AddParameterStoreConfiguration();
        }

        builder.BindServiceConfiguration();
        builder.UseSerilogAndDatadog();

        return builder;
    }

    public static WebApplicationBuilder UseSerilogAndDatadog(this WebApplicationBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(builder.Configuration["datadog:DATADOG_API_KEY"]))
        {
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .Enrich.WithProperty("dd_env", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("dd_version", builder.Configuration["TEMPLATE_VERSION"]!)
                .Enrich.WithClientIp()
                .Enrich.WithCorrelationId()
                .Enrich.WithRequestHeader("User-Agent")
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.DatadogLogs(builder.Configuration["datadog:DATADOG_API_KEY"],
                                     configuration: new DatadogConfiguration() { Url = "https://http-intake.logs.datadoghq.eu" },
                                     service: "template",
                                     host: Environment.MachineName)
            );
        }
        else
        {
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .Enrich.WithProperty("dd_env", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("dd_ver", builder.Configuration["TEMPLATE_VERSION"]!)
                .Enrich.WithClientIp()
                .Enrich.WithCorrelationId()
                .Enrich.WithRequestHeader("User-Agent")
                .WriteTo.Console(new CompactJsonFormatter())
            );
        }
        return builder;
    }

    public static WebApplicationBuilder AddParameterStoreConfiguration(this WebApplicationBuilder builder)
    {
        try
        {
            builder.Configuration.AddSystemsManager("/template/public");
            builder.Configuration.AddSystemsManager("/template/secret");
        }
        catch (Exception ex)
        {
            //Swallow this exception and rely on config from appconfig.json or somewhere else
            Console.WriteLine($"Exception while adding AWS Configuration: {ex.GetBaseException().Message}");
        }

        return builder;
    }

    public static void AddSecretsManagerConfiguration(this WebApplicationBuilder builder)
    {
        var secretsSection = builder.Configuration.GetSection("Secrets");

        foreach (var secretSection in secretsSection.GetChildren())
        {
            var sectionName = secretSection.Key;

            var secrets = secretSection.Get<string[]>()!;

            foreach (var secret in secrets)
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = secret,
                    VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
                };

                using var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName("eu-west-1"));
                try
                {
                    var response = client.GetSecretValueAsync(request).Result;

                    if (response.SecretString != null)
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString)!.First();
                        var section = builder.Configuration.GetSection(sectionName);
                        section[dict.Key] = dict.Value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving secret from AWS Secrets Manager: {ex.Message}");
                    // Handle or log the exception as needed.
                }
            }
        }
    }

    public static WebApplicationBuilder BindServiceConfiguration(this WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var optionsTypes = assembly.GetTypes()
            .Where(t => t.Namespace == $"{assembly.GetName().Name}.Options.Configurations" && !t.IsAbstract && t.IsClass);

        foreach (var optionsType in optionsTypes)
        {
            var sectionName = optionsType.Name;

            var optionsInstance = Activator.CreateInstance(optionsType);
            var section = builder.Configuration.GetSection(sectionName);

            section.Bind(optionsInstance);

            typeof(WebApplicationBuilderExtensions).GetMethod(nameof(BindDynamically))?.MakeGenericMethod(optionsType).Invoke(null, new object[] { builder, section });

            if (string.Equals(builder.Configuration["DumpConfigOnStartup"], "true", StringComparison.OrdinalIgnoreCase))
            {
                var serializedOptions = JsonSerializer.Serialize(optionsInstance, options: new JsonSerializerOptions() { WriteIndented = true });
                Console.WriteLine($"{optionsInstance!.GetType().Name}:");
                Console.WriteLine(serializedOptions);
            }
        }

        return builder;
    }

    public static void BindDynamically<T>(WebApplicationBuilder builder, IConfigurationSection section) where T : class => builder.Services.Configure<T>(section.Bind);
}
