using DocumentManagementStore.Common;

namespace DocumentManagementStore.Features.Documents
{
    public static class RenameDocumentFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapPost("/documents/{folderId}/rename", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<RenameDocument>>()
           .Produces<DocumentView>()
           .Produces(StatusCodes.Status400BadRequest);

        public static IResult Handle(Guid folderId)
        {
            return Results.Ok(new DocumentView(Guid.NewGuid()));
        }
        public record RenameDocument([FromRoute] Guid DocumentId, [FromBody] RenameDocument.RenameBody Payload)
        {
            public record RenameBody(string Name);
        }

        public record DocumentView(Guid DocumentId);

        public class Validator : AbstractValidator<RenameDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.Payload.Name).NotEmpty();
            }
        }

    }
}
