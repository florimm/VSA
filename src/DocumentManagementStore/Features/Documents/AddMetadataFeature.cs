using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain;
using DocumentManagementStore.Features.Documents.Domain;

namespace DocumentManagementStore.Features.Documents
{
    public static class AddMetadataFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapPost("/documents/{documentId}/metadata", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<AddMetadataToDocument>>()
           .Produces<DocumentView>()
           .Produces(StatusCodes.Status400BadRequest);

        public static async Task<IResult> Handle([AsParameters] AddMetadataToDocument req, [FromServices] IAggregateRepository aggregateRepository)
        {
            var doc = await aggregateRepository.LoadAsync<Document>(req.DocumentId);
            doc.AddMetadata(req.MetadataKey, req.MetadataValue);
            var result = await aggregateRepository.StoreAsync(doc);
            return Results.Ok(new DocumentView(doc.Id));
        }

        public record AddMetadataToDocument(string DocumentId, string MetadataKey, string MetadataValue);

        public record DocumentView(string DocumentId);

        public class Validator : AbstractValidator<AddMetadataToDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.MetadataKey).NotEmpty();
                RuleFor(x => x.MetadataValue).NotEmpty();
            }
        }

    }
}
