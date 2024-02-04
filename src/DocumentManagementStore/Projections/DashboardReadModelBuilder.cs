using Marten.Events.Projections;

namespace DocumentManagementStore.Projections;

public class DashboardReadModel
{
    public string Id { get; set; }
    public int NrOfFolders { get; set; }
    public int NrOfDocuments { get; set; }
}
public class DashboardReadModelBuilder : MultiStreamProjection<DashboardReadModel, string>
{
    
}