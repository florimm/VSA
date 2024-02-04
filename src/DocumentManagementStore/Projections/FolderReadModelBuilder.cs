using DocumentManagementStore.Domain.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace DocumentManagementStore.Projections;

public record FolderReadModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Documents { get; set; }
}

public class FolderReadModelBuilder : SingleStreamProjection<FolderReadModel>
{
    public void Apply(FolderCreated e, FolderReadModel model)
    {
        model.Id = e.Id;
        model.Name = e.Name;
        model.Documents = new List<string>();
    }
    public void Apply(FolderRenamed e, FolderReadModel model)
    {
        model.Name = e.NewName;
    }
}