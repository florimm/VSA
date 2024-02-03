namespace DocumentManagementStore.Features.Users;

public static class RevokeAccessFeature
{
    public record RevokeAccess(Guid FolderId, Guid UserId);

}