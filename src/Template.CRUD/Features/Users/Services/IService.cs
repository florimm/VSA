namespace Template.Api.Features.Users.V1;

public interface IUserService
{
    public Task<string?> GetProjectsByUserAsync(string email);
}
