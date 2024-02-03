using Marten.Events.Projections;

namespace DocumentManagementStore.Projections;

public record FolderReadModel
{
    public string Id { get; set; }
    public List<string> Documents { get; set; }
}

public class FolderReadModelBuilder : MultiStreamProjection<FolderReadModel, string>
{
}