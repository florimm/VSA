using DocumentManagementStore.Domain.Events;
using Marten.Events.Aggregation;

namespace DocumentManagementStore.Projections;

public class UserReadModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
}

public class UserReadModelBuilder : SingleStreamProjection<UserReadModel>
{
    public void Apply(UserRegistered e, UserReadModel model)
    {
        model.Id = e.Id;
        model.FullName = e.Name + " " + e.Surname;
        model.IsActive = true;
    }
    
    public void Apply(UserDeactivated e, UserReadModel model)
    {
        model.IsActive = false;
    }
}