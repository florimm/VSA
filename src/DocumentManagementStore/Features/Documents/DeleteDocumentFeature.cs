using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain;

namespace DocumentManagementStore.Features.Documents
{
    public static class DeleteDocumentFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapDelete("/documents/{documentId}", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<DeleteDocument>>()
           .Produces(StatusCodes.Status400BadRequest);

        public static async Task<IResult> Handle([AsParameters] DeleteDocument req, [FromServices] IAggregateRepository repo)
        {
            var document = await repo.LoadAsync<Document>(req.DocumentId);
            document.MarkAsDeleted();
            var events = await repo.StoreAsync(document);
            return Results.Ok();
        }

        public record DeleteDocument([FromRoute] string DocumentId);

        public class Validator : AbstractValidator<DeleteDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
            }
        }
    }
}
