using System.Reflection;

namespace DocumentManagementStore.Common;

public static class FeatureExtensions
{
    public static void LoadFeatures(this IEndpointRouteBuilder endpoints)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Get all static classes in the assembly
        var staticClasses = assembly
            .GetTypes()
            .Where(
                t =>
                    t.FullName!.Contains("DocumentManagementStore.Features")
                    && t.IsClass
                    && t.IsAbstract
                    && t.IsSealed
            )
            .ToList();

        // Get all static classes that methods that contain "Register"
        var endpointRouteBuilderClasses = staticClasses.Where(
            t => t.GetMethods().Any(r => r.Name.Contains("Register"))
        );

        foreach (var feature in endpointRouteBuilderClasses)
        {
            var staticFunctionInfo = feature.GetMethods()[0];
            staticFunctionInfo.Invoke(null, new object[] { endpoints });
        }
    }
}