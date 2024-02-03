namespace DocumentManagementStore.Features.Users;

public static class GrantAccessFeature
{
    public record GrantAccess(Guid EntityId, Guid UserId, string AccessType);

}