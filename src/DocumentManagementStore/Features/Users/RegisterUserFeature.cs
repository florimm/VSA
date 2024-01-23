namespace DocumentManagementStore.Features.Users
{
    public static class RegisterUserFeature
    {
        public record RegisterUser(Guid UserId, string Username, string Email, string Password);

    }

    public static class UpdateUserDetailsFeature
    {
        public record UpdateUserDetails(Guid UserId, string NewDetails);

    }

    public static class ChangeUserPasswordFeature
    {
        public record ChangeUserPassword(Guid UserId, string OldPassword, string NewPassword);

    }

    public static class DeactivateUserFeature
    {
        public record DeactivateUser(Guid UserId);

    }

    public static class GrantAccessFeature
    {
        public record GrantAccess(Guid EntityId, Guid UserId, string AccessType);

    }

    public static class RevokeAccessFeature
    {
        public record RevokeAccess(Guid EntityId, Guid UserId);

    }

    public static class UpdateAccessLevelFeature
    {
        public record UpdateAccessLevel(Guid EntityId, Guid UserId, string NewAccessType);

    }
}
