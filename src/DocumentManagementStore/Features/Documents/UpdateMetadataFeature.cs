using DocumentManagementStore.Common;

namespace DocumentManagementStore.Features.Documents
{
    public static class UpdateMetadataFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapPut("/documents/{documentId}/metadata", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<UpdateDocumentMetadata>>()
           .Produces<DocumentView>()
           .Produces(StatusCodes.Status400BadRequest);
        public static IResult Handle(Guid folderId)
        {
            return Results.Ok(new DocumentView(Guid.NewGuid()));
        }
        public record UpdateDocumentMetadata(Guid DocumentId, string MetadataKey, string NewMetadataValue);

        public record DocumentView(Guid DocumentId);

        public class Validator : AbstractValidator<UpdateDocumentMetadata>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.MetadataKey).NotEmpty();
                RuleFor(x => x.NewMetadataValue).NotEmpty();
            }
        }

    }
}
