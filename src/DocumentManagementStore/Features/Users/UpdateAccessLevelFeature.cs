namespace DocumentManagementStore.Features.Users;

public static class UpdateAccessLevelFeature
{
    public record UpdateAccessLevel(Guid EntityId, Guid UserId, string NewAccessType);
}