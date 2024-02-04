using DocumentManagementStore.Common;

namespace DocumentManagementStore.Features.Folders
{
    public static class CreateFolderFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
            endpoints
                .MapPost("/folders", Handle)
                .WithTags("folders")
                .AddEndpointFilter<ValidationFilter<CreateFolder>>()
                .Produces<FolderView>()
                .Produces(StatusCodes.Status400BadRequest);

        public static Task<IResult> Handle()
        {
            return Task.FromResult(Results.Ok());
        }

        public record CreateFolder();

        public record FolderView();
        
        public class Validator : AbstractValidator<CreateFolder>
        {
            public Validator()
            {
            }
        }
    }
}
