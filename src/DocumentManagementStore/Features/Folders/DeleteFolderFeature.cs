using DocumentManagementStore.Common;

namespace DocumentManagementStore.Features.Folders
{
    public static class DeleteFolderFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
            endpoints
                .MapPost("/folders", Handle)
                .WithTags("folders")
                .AddEndpointFilter<ValidationFilter<DeleteFolder>>()
                .Produces(StatusCodes.Status400BadRequest);

        public static Task<IResult> Handle()
        {
            return Task.FromResult(Results.Ok());
        }

        public record DeleteFolder();
        
        public class Validator : AbstractValidator<DeleteFolder>
        {
            public Validator()
            {
            }
        }
    }
}
