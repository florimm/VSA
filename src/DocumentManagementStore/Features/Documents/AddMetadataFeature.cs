using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
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

        public static async Task<IResult> Handle(Guid folderId, [FromServices] IAggregateRepository aggregateRepository)
        {
            var result = await aggregateRepository.StoreAsync(new Document());
            return Results.Ok(new DocumentView(Guid.NewGuid()));
        }

        public record AddMetadataToDocument(Guid DocumentId, string MetadataKey, string MetadataValue);

        public record DocumentView(Guid DocumentId);

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
