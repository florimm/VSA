using Marten.Pagination;
using Marten;
using DocumentManagementStore.Common.Core.Exceptions;

namespace DocumentManagementStore.Common.Core.ES.Repositories;

public interface IViewRepository
{
    Task<T> Load<T>(string id, CancellationToken cancellationToken)
        where T : class, new();

    Task<IReadOnlyList<T>> LoadAll<T>(int skip, int take, CancellationToken cancellationToken)
        where T : class, new();

    Task<IPagedList<T>> Load<T>(int pageNumber, int pageSize, CancellationToken cancellationToken)
        where T : class, new();
}

public class ViewRepository : IViewRepository
{
    private readonly IDocumentStore _store;

    public ViewRepository(IDocumentStore store) => _store = store;

    public async Task<T> Load<T>(string id, CancellationToken cancellationToken)
        where T : class, new()
    {
        using var session = _store.QuerySession();
        var view =
            await session.LoadAsync<T>(id, cancellationToken)
            ?? throw new ViewDoesNotExistException("View does not exist");
        return view;
    }

    public async Task<IReadOnlyList<T>> LoadAll<T>(
        int skip,
        int take,
        CancellationToken cancellationToken
    )
        where T : class, new()
    {
        using var session = _store.QuerySession();
        var items =
            await session.Query<T>().Skip(skip).Take(take).ToListAsync(cancellationToken)
            ?? throw new ViewDoesNotExistException("View does not exist");
        return items;
    }

    public async Task<IPagedList<T>> Load<T>(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    )
        where T : class, new()
    {
        using var session = _store.QuerySession();
        var items =
            await session.Query<T>().ToPagedListAsync(pageNumber, pageSize, cancellationToken)
            ?? throw new ViewDoesNotExistException("View does not exist");
        return items;
    }
}
