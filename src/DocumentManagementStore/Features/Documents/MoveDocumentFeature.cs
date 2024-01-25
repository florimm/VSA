using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain;

namespace DocumentManagementStore.Features.Documents
{
    public static class MoveDocumentFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapPost("/documents/{documentId}/move/{folderId}", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<MoveDocument>>()
           .Produces(StatusCodes.Status400BadRequest);

        public static async Task<IResult> Handle([AsParameters] MoveDocument req, [FromServices] IAggregateRepository repo)
        {
            var document = await repo.LoadAsync<Document>(req.DocumentId);
            document.MoveDocument(req.NewFolderId);
            var events = await repo.StoreAsync(document);
            return Results.Ok();
        }

        public record MoveDocument([FromRoute] string DocumentId, [FromRoute] string NewFolderId);

        public class Validator : AbstractValidator<MoveDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.NewFolderId).NotEmpty();
            }
        }

    }
}
