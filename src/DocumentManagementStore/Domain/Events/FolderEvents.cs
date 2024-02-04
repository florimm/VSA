namespace DocumentManagementStore.Domain.Events;

public record FolderCreated(string Id, string Name);
public record FolderRenamed(string Id, string NewName);