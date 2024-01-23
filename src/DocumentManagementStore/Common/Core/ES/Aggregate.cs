using DocumentManagementStore.Common.Core.Events;
using System.Reflection;
using System.Text.Json.Serialization;

namespace DocumentManagementStore.Common.Core.ES;
public interface IAggregate
{
    string Id { get; }
    int Version { get; }

    object[] DequeueUncommittedEvents();
}

public abstract class Aggregate
{
    // For indexing our event streams
    public string Id { get; protected set; }

    // For protecting the state, i.e. conflict prevention
    // The setter is only public for setting up test conditions
    public long Version { get; set; }

    // JsonIgnore - for making sure that it won't be stored in inline projection
    [JsonIgnore] private readonly List<object> _uncommittedEvents = new List<object>();

    // Get the deltas, i.e. events that make up the state, not yet persisted
    public IEnumerable<object> GetUncommittedEvents()
    {
        return _uncommittedEvents;
    }

    // Mark the deltas as persisted.
    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    protected void AddUncommittedEvent(object @event)
    {
        // add the event to the uncommitted list
        _uncommittedEvents.Add(@event);
    }

    protected void ApplyAndCommitEvents<T>(T @event)
    {
        AddUncommittedEvent(@event);
    }

    protected void ApplyEvent(IDomainEvent @event)
    {
        var methodName = "Apply";
        var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { @event.GetType() }, null);
        if (method == null)
            throw new NotImplementedException($"Method 'private void {methodName}({@event.GetType().Name} @event)' is not found in {GetType().Name} aggregate");

        method.Invoke(this, new object[] { @event });
        AddUncommittedEvent(@event);
    }
}
