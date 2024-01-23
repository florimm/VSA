using DocumentManagementStore.Common;

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

        public static IResult Handle([AsParameters] MoveDocument request)
        {
            return Results.Ok();
        }

        public record MoveDocument([FromRoute] Guid DocumentId, [FromRoute] Guid NewFolderId);

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
