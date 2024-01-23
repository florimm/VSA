using DocumentManagementStore.Common;

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
        public static IResult Handle(Guid folderId)
        {
            return Results.Ok();
        }
        public record RemoveMetadataFromDocument(Guid DocumentId, string MetadataKey);

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
