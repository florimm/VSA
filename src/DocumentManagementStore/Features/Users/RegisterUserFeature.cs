using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.ES.Repositories;
using DocumentManagementStore.Domain;

namespace DocumentManagementStore.Features.Users
{
    public static class RegisterUserFeature
    {
        public static void Register(IEndpointRouteBuilder endpoints) =>
            endpoints
                .MapPost("/users", Handle)
                .WithTags("users")
                .AddEndpointFilter<ValidationFilter<RegisterUser>>()
                .Produces<UserView>()
                .Produces(StatusCodes.Status400BadRequest);

        public static async Task<IResult> Handle([AsParameters]RegisterUser req, [FromServices] IAggregateRepository aggregateRepository)
        {
            var doc = new User();
            await aggregateRepository.StoreAsync(doc);
            return Results.Ok(new UserView(doc.Id));
        }
        public record RegisterUser(Guid UserId, string Username, string Email, string Password);

        public record UserView(string Id);

    }
}
