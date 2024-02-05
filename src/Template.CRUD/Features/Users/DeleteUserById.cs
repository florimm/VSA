namespace Template.Api.Features.Users.V1.Handlers;

public static class DeleteUserByIdFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
    endpoints
        .MapDelete("/api/users/{id}", Handle)
        .WithTags("users")
        .Produces(StatusCodes.Status400BadRequest);

    public static async Task<IResult> Handle([AsParameters] DeleteUserCommand payload, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(payload);
        return Results.Ok(result);
    }
}
public record DeleteUserCommand([FromRoute] string Id) : IRequest<Result>;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IDatabaseContext _userDbContext;

    public DeleteUserHandler(IDatabaseContext userDbContext) => _userDbContext = userDbContext;

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userDbContext.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        if (user == null)
        {
            return Result.Failed(new ErrorResponse($"User {request.Id} not found", "USER_NOT_FOUND"));
        }
        _userDbContext.Users.Remove(user);
        await _userDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
