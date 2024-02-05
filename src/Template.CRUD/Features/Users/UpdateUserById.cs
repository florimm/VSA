using Template.Api.Features.Users.Models;

namespace Template.Api.Features.Users.V1.Handlers;

public static class UpdateUserFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
    endpoints
        .MapPut("/api/users/{id}", Handle)
        .WithTags("users")
        .Produces<UserView>()
        .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([FromBody] UpdateUserByIdCommand payload, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(payload);
        return Results.Ok(result);
    }
    public record UpdateUserByIdCommand([FromRoute] string Id, [FromBody] UpdateUserPayload Payload) : IRequest<Result<UserView>>;

    public record UpdateUserPayload(
        string? City,
        string? Country,
        string? CountryCode,
        string? FirstName,
        string? LastName,
        string? Language,
        string? PersonalNumber,
        string? PhoneNumber,
        string? PostalCode,
        string? Role,
        string? StreetAddress,
        DateTime? DeletedAt
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

    public class UpdateUserByIdHandler : IRequestHandler<UpdateUserByIdCommand, Result<UserView>>
    {
        private readonly IDatabaseContext _userDbContext;

        public UpdateUserByIdHandler(IDatabaseContext userDbContext) => _userDbContext = userDbContext;

        public async Task<Result<UserView>> Handle(UpdateUserByIdCommand request, CancellationToken cancellationToken)
        {
            var user = await _userDbContext.Users.FindAsync(new object[] { request.Id }, cancellationToken);

            // Update the user
            user!.City = request.Payload.City ?? user.City;
            user.Country = request.Payload.Country ?? user.Country;
            user.CountryCode = request.Payload.CountryCode ?? user.CountryCode; // TODO find country by code
            user.DeletedAt = request.Payload.DeletedAt ?? user.DeletedAt;
            user.FirstName = request.Payload.FirstName ?? user.FirstName;
            user.Language = request.Payload.Language ?? user.Language;
            user.LastName = request.Payload.LastName ?? user.LastName;
            user.PersonalNumber = request.Payload.PersonalNumber ?? user.PersonalNumber;
            user.PhoneNumber = request.Payload.PhoneNumber ?? user.PhoneNumber;
            user.PostalCode = request.Payload.PostalCode ?? user.PostalCode;
            user.Role = request.Payload.Role != null ? (UserRole)Enum.Parse(typeof(UserRole), request.Payload.Role, true) : user.Role;
            user.StreetAddress = request.Payload.StreetAddress ?? user.StreetAddress;
            user.UpdatedAt = DateTime.UtcNow;

            await _userDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                new UserView(
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
                )
            );
        }
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUserByIdCommand>
    {
        private readonly string _regexStrAlphabetic = @"^^[a-zA-Z\s]+$";
        private readonly IDatabaseContext _database;

        public UpdateUserValidator(IDatabaseContext database)
        {
            _database = database;
            RuleFor(_ => _.Id).MustAsync(MustExist).WithMessage("User must exist!");
            RuleFor(_ => _.Payload.FirstName).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.Payload.FirstName));
            RuleFor(_ => _.Payload.LastName).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.Payload.LastName));
            RuleFor(_ => _.Payload.City).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.Payload.City));
            RuleFor(_ => _.Payload.Country).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.Payload.Country));
            RuleFor(_ => _.Payload.CountryCode).Matches(_regexStrAlphabetic).When(_ => !string.IsNullOrEmpty(_.Payload.CountryCode));
        }

        private async Task<bool> MustExist(string id, CancellationToken token)
        {
            var user = await _database.Users.FindAsync(new object[] { id }, token);
            return user is not null;
        }
    }
}
