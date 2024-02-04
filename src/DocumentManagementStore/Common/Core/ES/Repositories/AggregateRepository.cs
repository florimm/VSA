using DocumentManagementStore.Common.Core.Events;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Logging;

namespace DocumentManagementStore.Common.Core.ES.Repositories;

public record SavedResult(bool SavedSuccess, IEnumerable<object>? Events);

public interface IAggregateRepository
{
    Task<SavedResult> StoreAsync(Aggregate aggregate, CancellationToken ct = default);

    Task<SavedResult> StoreAsync(string streamId, IDomainEvent @event, CancellationToken ct = default);

    Task<T> LoadAsync<T>(string id, int? version = null, CancellationToken ct = default)
        where T : Aggregate;

    Task<IReadOnlyList<IEvent>> LoadEvents(string id, CancellationToken ct = default);
}

public class AggregateRepository : IAggregateRepository
{
    private readonly IDocumentStore _store;
    private readonly ILogger<AggregateRepository> _logger;

    public AggregateRepository(IDocumentStore store, ILogger<AggregateRepository> logger)
    {
        _store = store;
        _logger = logger;
    }

    public async Task<T> LoadAsync<T>(
        string id,
        int? version = null,
        CancellationToken ct = default
    )
        where T : Aggregate
    {
        using var session = _store.LightweightSession();
        var aggregate = await session.Events.AggregateStreamAsync<T>(id, token: ct);

        return aggregate ?? throw new InvalidOperationException($"No aggregate by id {id}.");
    }

    public async Task<IReadOnlyList<IEvent>> LoadEvents(string id, CancellationToken ct = default)
    {
        using var session = _store.LightweightSession();
        var events = await session.Events.FetchStreamAsync(id, token: ct);

        return events;
    }

    public async Task<SavedResult> StoreAsync(Aggregate aggregate, CancellationToken ct = default)
    {
        try
        {
            await using var session = _store.LightweightSession();
            var events = aggregate.GetUncommittedEvents().ToList();
            session.Events.Append(aggregate.Id, aggregate.Version, events);
            await session.SaveChangesAsync(ct);
            aggregate.ClearUncommittedEvents();
            return new SavedResult(true, events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new SavedResult(false, null);
        }
    }

    public async Task<SavedResult> StoreAsync(
        string streamId,
        IDomainEvent @event,
        CancellationToken ct = default
    )
    {
        try
        {
            using var session = _store.LightweightSession();
            session.Events.Append(streamId, @event);
            await session.SaveChangesAsync(ct);
            return new SavedResult(true, [@event]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new SavedResult(false, null);
        }
    }
}
