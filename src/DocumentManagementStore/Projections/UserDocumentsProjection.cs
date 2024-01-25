using DocumentManagementStore.Domain.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace DocumentManagementStore.Projections
{
    public record DocumentReadModel(string DocumentId);

    public class DocumentReadModelBuilder : MultiStreamProjection<DocumentReadModel, string>
    {
    }

    public record UserReadModel();

    public class UserReadModelBuilder : SingleStreamProjection<UserReadModel>
    {
    }
}
