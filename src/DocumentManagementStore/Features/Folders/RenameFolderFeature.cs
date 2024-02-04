using DocumentManagementStore.Common;

namespace DocumentManagementStore.Features.Folders
{
    public static class RenameFolderFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
            endpoints
                .MapPost("/folders", Handle)
                .WithTags("folders")
                .AddEndpointFilter<ValidationFilter<RenameFolder>>()
                .Produces(StatusCodes.Status400BadRequest);

        public static Task<IResult> Handle()
        {
            return Task.FromResult(Results.Ok());
        }

        public record RenameFolder();
        
        public class Validator : AbstractValidator<RenameFolder>
        {
            public Validator()
            {
            }
        }
    }
}
