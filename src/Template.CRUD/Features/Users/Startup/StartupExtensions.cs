namespace Template.Api.Features.Users.V1.Startup;

public static class StartupExtensions
{

    public static void Configure(IServiceCollection services) => services.AddTransient<IUserService, UserService>();
}
