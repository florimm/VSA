using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain.Events;
using DocumentManagementStore.Features.Documents;
using Marten.Events.Projections;

namespace DocumentManagementStore.Features.Users;

public static class GetDocumentsByUsersFeatures
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
    endpoints
        .MapPost("/users/{userId}/documents", Handle)
        .WithTags("documents")
        .AddEndpointFilter<ValidationFilter<GetDocumentByUserId>>()
        .Produces<IReadOnlyList<DocumentByUser>>()
        .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([FromServices] IViewRepository repo, CancellationToken token)
    {
        var result = await repo.LoadAll<DocumentByUser>(0, 100, token);
        return Results.Ok(result);
    }

    public record GetDocumentByUserId([FromRoute] string UserId);

    public record DocumentByUser(string DocumentId, string UserId, string DocumentName);
}
