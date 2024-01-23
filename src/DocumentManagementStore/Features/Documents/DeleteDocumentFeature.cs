using DocumentManagementStore.Common;

namespace DocumentManagementStore.Features.Documents
{
    public static class DeleteDocumentFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
       endpoints
           .MapDelete("/documents", Handle)
           .WithTags("documents")
           .AddEndpointFilter<ValidationFilter<DeleteDocument>>()
           .Produces(StatusCodes.Status400BadRequest);

        public static IResult Handle()
        {
            return Results.Ok();
        }

        public record DeleteDocument(Guid DocumentId);

    }
}
