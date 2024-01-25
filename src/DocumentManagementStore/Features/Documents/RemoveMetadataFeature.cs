using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain;

namespace DocumentManagementStore.Features.Documents
{
    public class RemoveMetadataFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapDelete("/documents/{documentId}/metadata", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<RemoveMetadataFromDocument>>()
           .Produces(StatusCodes.Status400BadRequest);
        
        public static async Task<IResult> Handle([AsParameters] RemoveMetadataFromDocument req, [FromServices] IAggregateRepository repo)
        {
            var document = await repo.LoadAsync<Document>(req.DocumentId);
            document.RemoveMetadata(req.MetadataKey);
            var events = await repo.StoreAsync(document);
            return Results.Ok();
        }
        public record RemoveMetadataFromDocument(string DocumentId, string MetadataKey);

        public class Validator : AbstractValidator<RemoveMetadataFromDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.MetadataKey).NotEmpty();
            }
        }

    }
}
