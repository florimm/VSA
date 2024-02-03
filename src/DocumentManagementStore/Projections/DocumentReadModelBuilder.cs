using Marten.Events.Projections;

namespace DocumentManagementStore.Projections
{
    public record DocumentReadModel(string DocumentId);

    public class DocumentReadModelBuilder : MultiStreamProjection<DocumentReadModel, string>
    {
    }
}
