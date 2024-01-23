using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Features.Documents.Domain;

namespace DocumentManagementStore.Features.Documents
{
    public static class CreateDocumentFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapPost("/documents", Handle)
            .WithTags("documents")
            .AddEndpointFilter<ValidationFilter<CreateDocument>>()
            .Produces<DocumentView>()
            .Produces(StatusCodes.Status400BadRequest);

        public static async Task<IResult> Handle(IAggregateRepository aggregateRepository)
        {
            var doc = new Document() { };
            await aggregateRepository.StoreAsync(doc);
            return Results.Ok(new DocumentView(Guid.NewGuid()));
        }

        public record CreateDocument(Guid DocumentId, string Title, string Content, Guid AuthorId, Guid? FolderId);

        public record DocumentView(Guid DocumentId);


        public class Validator : AbstractValidator<CreateDocument>
        {
            public Validator()
            {
                RuleFor(x => x.DocumentId).NotEmpty();
                RuleFor(x => x.Title).MinimumLength(3).NotEmpty();
                RuleFor(x => x.Content).MinimumLength(3).NotEmpty();
                RuleFor(x => x.AuthorId).NotEmpty();
                RuleFor(x => x.FolderId).NotEmpty();
            }
        }

    }
}
