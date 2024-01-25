using DocumentManagementStore.Common.Core.Events;

namespace DocumentManagementStore.Domain.Events;

public record DocumentCreated(string Id) : IDomainEvent;
public record DocumentDeleted(string Id) : IDomainEvent;
public record DocumentMoved(string Id, string NewFolderId) : IDomainEvent;

public record DocumentRenamed(string Id, string Name) : IDomainEvent;

public record DocumentMetadataAdded(string Id, string Key, string Value) : IDomainEvent;
public record DocumentMetadataRemoved(string Id, string Key) : IDomainEvent;