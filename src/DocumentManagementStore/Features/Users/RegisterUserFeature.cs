namespace DocumentManagementStore.Features.Users
{
    public static class RegisterUserFeature
    {
        public record RegisterUser(Guid UserId, string Username, string Email, string Password);

    }
}
