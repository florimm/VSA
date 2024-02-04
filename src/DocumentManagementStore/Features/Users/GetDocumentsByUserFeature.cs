using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Projections;

namespace DocumentManagementStore.Features.Users;

public static class GetDocumentsByUsersFeatures
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
    endpoints
        .MapPost("/users/{userId}/documents", Handle)
        .WithTags("users")
        .AddEndpointFilter<ValidationFilter<GetDocumentByUserId>>()
        .Produces<IReadOnlyList<DocumentByUser>>()
        .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([FromServices] IViewRepository repo, CancellationToken token)
    {
        var result = await repo.LoadAll<UserReadModel>(0, 100, token);
        var response = result.Select(t => t.ToView());
        return Results.Ok(response);
    }

    public static DocumentByUser ToView(this UserReadModel model)
    {
        return new DocumentByUser("", "", "");
    }

    public record GetDocumentByUserId([FromRoute] string UserId);

    public record DocumentByUser(string DocumentId, string UserId, string DocumentName);
}
