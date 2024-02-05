using FluentValidation.Results;

namespace Template.Api.Common.Error;

public class ErrorResponse
{
    public bool IsValidationError { get; }
    public string Message { get; }
    public string? ErrorCode { get; }

    public ErrorResponse(string message, string? errorCode = null, bool isValidationError = false)
    {
        IsValidationError = isValidationError;
        Message = message;
        ErrorCode = errorCode;
    }
}

public class ValidationError : ErrorResponse
{

    public ValidationError(List<ValidationFailure> validationErrors) : base(string.Join(",", validationErrors), null, true)
    {

    }
}
