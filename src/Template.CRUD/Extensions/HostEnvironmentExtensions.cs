namespace Template.Api.Extensions;
public static class HostEnvironmentExtensions
{
    public static bool IsLocal(this IHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("Local");
}
