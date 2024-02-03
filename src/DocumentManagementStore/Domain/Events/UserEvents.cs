using DocumentManagementStore.Common.Core.Events;

namespace DocumentManagementStore.Domain.Events;

public record UserRegistered(string Id) : IDomainEvent;

public record AccessGranted(string Id, string FolderId) : IDomainEvent;

public record UserDeactivated(string Id) : IDomainEvent;