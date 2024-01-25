using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain;

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

        public static async Task<IResult> Handle([AsParameters] RenameDocument req, [FromServices] IAggregateRepository repo)
        {
            var document = await repo.LoadAsync<Document>(req.DocumentId);
            document.Rename(req.Payload.Name);
            var events = await repo.StoreAsync(document);
            return Results.Ok(document.ToView());
        }
        public record RenameDocument([FromRoute] string DocumentId, [FromBody] RenameDocument.RenameBody Payload)
        {
            public record RenameBody(string Name);
        }

        public record DocumentView(string DocumentId);

        public class Validator : AbstractValidator<RenameDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.Payload.Name).NotEmpty();
            }
        }

        public static DocumentView ToView(this Document doc)
        {
            return new DocumentView(doc.Id);
        }
    }
}
