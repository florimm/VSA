namespace Template.Api.Common.Pipelines;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var errors = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var item in _validators)
        {
            var result = await item.ValidateAsync(request, cancellationToken);
            if (result.Errors.Any())
            {
                errors.AddRange(result.Errors);
            }
        }

        if (errors.Any())
        {
            throw new FluentValidation.ValidationException(errors);
        }

        return await next();
    }
}
