using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Features.Documents.Domain;

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

        public static async Task<IResult> Handle([FromServices] IAggregateRepository repo)
        {
            var document = await repo.LoadAsync<Document>("");
            document.MarkAsDeleted();
            var events = await repo.StoreAsync(document);
            return Results.Ok();
        }

        public record DeleteDocument(Guid DocumentId);

    }
}
