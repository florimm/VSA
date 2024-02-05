namespace Template.Api.Common.Health.DaprServiceHealth;

using System.Globalization;
using System.Net.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class GenericDaprServiceHealth : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _httpClientName;
    private readonly bool _degradedInsteadOfUnhealthy;

    public GenericDaprServiceHealth(IHttpClientFactory httpClientFactory, string httpClientName, bool degradedInsteadOfUnhealthy)
    {
        _httpClientFactory = httpClientFactory;
        _httpClientName = httpClientName;
        _degradedInsteadOfUnhealthy = degradedInsteadOfUnhealthy;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var errors = new StringBuilder();
        var info = new StringBuilder();
        var testPath = "health/live";

        try
        {
            var httpClient = _httpClientFactory.CreateClient(_httpClientName);
            var daprAppId = httpClient.DefaultRequestHeaders.Any(kv => kv.Key == "dapr-app-id") ? httpClient.DefaultRequestHeaders.GetValues("dapr-app-id").FirstOrDefault() : null;

            info.Append(CultureInfo.InvariantCulture, $"Uri is ({httpClient.BaseAddress}{testPath}). ");
            info.Append(CultureInfo.InvariantCulture, $"dapr-app-id header is ({daprAppId}). ");

            if (string.IsNullOrWhiteSpace(daprAppId))
            {
                errors.Append(CultureInfo.InvariantCulture, $"Missing dapr-app-id header in http client for {_httpClientName}. ");
            }

            var response = await httpClient.GetAsync(testPath);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                errors.Append(CultureInfo.InvariantCulture, $"Unexpected response from {_httpClientName}. {response.StatusCode} - {response.ReasonPhrase}. ");
            }
        }
        catch (Exception ex)
        {
            return _degradedInsteadOfUnhealthy ? HealthCheckResult.Degraded($"{_httpClientName} unavailable. {info}{errors}", ex) : HealthCheckResult.Unhealthy($"{_httpClientName} unavailable. {info}{errors}", ex);
        }

        if (errors.Length > 0)
        {
            return _degradedInsteadOfUnhealthy ? HealthCheckResult.Degraded($"{_httpClientName} unavailable. {info}{errors}") : HealthCheckResult.Unhealthy($"{_httpClientName} unavailable. {info}{errors}");
        }
        else
        {
            return HealthCheckResult.Healthy($"{_httpClientName} is accessible. {info}");
        }
    }
}
