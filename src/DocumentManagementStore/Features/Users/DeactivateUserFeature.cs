namespace DocumentManagementStore.Features.Users;

public static class DeactivateUserFeature
{
    public record DeactivateUser(Guid UserId);

}