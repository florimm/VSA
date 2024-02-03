namespace DocumentManagementStore.Features.Users;

public static class RevokeAccessToDocumentFeature
{
    public record RevokeAccess(Guid FolderId, Guid DocumentId, Guid UserId);
}