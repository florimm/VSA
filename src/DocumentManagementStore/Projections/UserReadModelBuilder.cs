using DocumentManagementStore.Domain.Events;
using Marten.Events.Aggregation;

namespace DocumentManagementStore.Projections;

public record UserReadModel(string UserId, string FullName);

public class UserReadModelBuilder : SingleStreamProjection<UserReadModel>
{
    public void Apply(UserRegistered e, UserReadModel trip)
    {
    }
    
    public void Apply(UserDeactivated e, UserReadModel trip)
    {
    }
}