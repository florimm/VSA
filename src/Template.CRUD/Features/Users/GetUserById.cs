namespace Template.Api.Features.Users.V1.Handlers;


public static class GetUserByIdFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
   endpoints
       .MapGet("/api/users/{userId}", Handle)
       .WithTags("users")
       .Produces<UserView>()
       .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([AsParameters] GetUserByIdQuery payload, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(payload);
        return Results.Ok(result);
    }

    public record GetUserByIdQuery(string Id) : IRequest<Result<UserView>>;

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

    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserView>>
    {
        private readonly IDatabaseContext _userDbContext;

        public GetUserByIdHandler(IDatabaseContext userDbContext) => _userDbContext = userDbContext;

        public async Task<Result<UserView>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userDbContext.Users.FindAsync(new object[] { request.Id }, cancellationToken);
            if (user == null)
            {
                return Result.Failed<UserView>(new ErrorResponse($"User {request.Id} not found", "USER_NOT_FOUND"));
            }
            return Result.Ok(new UserView(
                user.Id,
                user.City,
                user.Country,
                user.CountryCode,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Language,
                user.PersonalNumber,
                user.PhoneNumber,
                user.PostalCode,
                user.Role.ToString(),
                user.StreetAddress,
                user.CreatedAt,
                user.DeletedAt,
                user.UpdatedAt
            ));
        }
    }
}
