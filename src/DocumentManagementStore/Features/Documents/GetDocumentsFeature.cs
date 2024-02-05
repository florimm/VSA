using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Projections;

namespace DocumentManagementStore.Features.Documents;
public static class GetDocumentsFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
    endpoints
        .MapGet("/documents", Handle)
        .WithTags("documents")
        .Produces<IReadOnlyList<DocumentView>>()
        .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([FromServices] IViewRepository repo, CancellationToken token)
    {
        var result = await repo.LoadAll<DocumentReadModel>(0, 100, token);
        var response = result.Select(t => t.ToView());
        return Results.Ok(response);
    }

    public static DocumentView ToView(this DocumentReadModel model)
    {
        return new DocumentView(model.Id, model.Name);
    }

    public record DocumentView(string DocumentId, string Name);
}

