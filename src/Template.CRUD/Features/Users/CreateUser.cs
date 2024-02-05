using Template.Api.Features.Users.Models;

namespace Template.Api.Features.Users.V1.Handlers;

public static class CreateUserFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
    endpoints
        .MapPost("/api/users", Handle)
        .WithTags("users")
        .Produces<UserView>()
        .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([AsParameters] CreateUserCommand payload, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(payload);
        return Results.Ok(result);
    }

    public record CreateUserCommand([FromBody] CreateUserPayload Payload) : IRequest<Result<UserView>>;

    public record CreateUserPayload(
        string Id,
        string? City,
        string? Country,
        string? CountryCode,
        string Email,
        string? FirstName,
        string? LastName,
        string? Language,
        string? PersonalNumber,
        string? PhoneNumber,
        string? PostalCode,
        string Role,
        string? StreetAddress
    );

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

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<UserView>>
    {
        private readonly IDatabaseContext _userDbContext;

        public CreateUserHandler(IDatabaseContext userDbContext) => _userDbContext = userDbContext;

        public async Task<Result<UserView>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var newUser = new UserModel()
            {
                Id = request.Payload.Id,
                City = request.Payload.City,
                Country = request.Payload.Country,
                CountryCode = request.Payload.CountryCode,
                Email = request.Payload.Email,
                FirstName = request.Payload.FirstName,
                Language = request.Payload.Language,
                LastName = request.Payload.LastName,
                PersonalNumber = request.Payload.PersonalNumber,
                PhoneNumber = request.Payload.PhoneNumber,
                PostalCode = request.Payload.PostalCode,
                Role = (UserRole)Enum.Parse(typeof(UserRole), request.Payload.Role, true),
                StreetAddress = request.Payload.StreetAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var user = _userDbContext.Users.Add(newUser).Entity;
            await _userDbContext.SaveChangesAsync(cancellationToken);

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

    public class CreateUserValidator : AbstractValidator<CreateUserPayload>
    {
        private readonly string _regexStrAlphabetic = @"^^[a-zA-Z\s]+$";
        private readonly string _regexGUID = @"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$";
        private readonly IDatabaseContext _databaseContext;

        public CreateUserValidator(IDatabaseContext databaseContext)
        {
            RuleFor(_ => _.Id).Matches(_regexGUID);
            RuleFor(_ => _.Email).EmailAddress();
            RuleFor(_ => _.Email).MustAsync(NotExist).WithMessage("Email already exist!");
            RuleFor(_ => _.FirstName).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.FirstName));
            RuleFor(_ => _.LastName).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.LastName));
            RuleFor(_ => _.City).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.City));
            RuleFor(_ => _.Country).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.Country));
            RuleFor(_ => _.CountryCode).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.CountryCode));
            _databaseContext = databaseContext;
        }

        private async Task<bool> NotExist(string email, CancellationToken token)
        {
            var result = await _databaseContext.Users.Where(x => x.Email == email).CountAsync(token);
            return result switch
            {
                > 0 => false,
                _ => true
            };
        }
    }
}
