
namespace Template.Api.Features.Users.V1.Handlers;
using ResultType = Result<string>;

public static class GetProjectByUserFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
   endpoints
       .MapGet("/api/users/{userId}/projects", Handle)
       .WithTags("users")
       .Produces<UserView>()
       .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([AsParameters] GetProjectByUserCommand payload, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(payload);
        return Results.Ok(result);
    }
    public record GetProjectByUserCommand([FromRoute] string UserId) : IRequest<ResultType>;

    public record UserView(
        string Id,
        string? City,
        string? Country,
        string? CountryCode,
        string? Email,
        string? FirstName,
        string? LastName,
        string? Language,
        string? PersonalNumber,
        string? PhoneNumber,
        string? PostalCode,
        string Role,
        string? StreetAddress,
        DateTime CreatedAt,
        DateTime? DeletedAt,
        DateTime UpdatedAt
    );

    public class GetProjectByUserHandler : IRequestHandler<GetProjectByUserCommand, ResultType>
    {
        private readonly IUserService _userService;

        public GetProjectByUserHandler(IUserService userService) => _userService = userService;

        public async Task<ResultType> Handle(GetProjectByUserCommand request, CancellationToken cancellationToken)
        {
            var res = await _userService.GetProjectsByUserAsync(request.UserId);
            if (res is not null)
            {
                return Result.Ok(res);
            }
            return Result.Failed<string>(new ErrorResponse("could not activate product"));
        }

    }
}


