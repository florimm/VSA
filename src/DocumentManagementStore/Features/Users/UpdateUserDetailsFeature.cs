namespace DocumentManagementStore.Features.Users;

public static class UpdateUserDetailsFeature
{
    public record UpdateUserDetails(Guid UserId, string NewDetails);

}