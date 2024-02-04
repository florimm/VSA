using DocumentManagementStore.Domain.Events;
using Marten.Events.Projections;

namespace DocumentManagementStore.Projections;

public class UserFoldersReadModel
{
    public string Id { get; set; }
    public List<string> Folders { get; set; } = new List<string>();
}

public class UserFoldersReadModelBuilder : MultiStreamProjection<UserFoldersReadModel, string>
{
    public UserFoldersReadModelBuilder()
    {
        this.Identity<UserRegistered>(c => c.Id);
        this.Identity<AccessGranted>(c => c.Id);
    }
        
    public void Apply(UserRegistered @event, UserFoldersReadModel model)
        => model.Id = @event.Id;

    public void Apply(AccessGranted @event, UserFoldersReadModel model)
    {
        model.Id = @event.Id;
        model.Folders.Add(@event.FolderId);
    }
}