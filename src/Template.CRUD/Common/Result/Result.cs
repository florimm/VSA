
namespace Template.Api.Common.Result;

public class Result
{
    public bool Success { get; }

    public ErrorResponse? Error { get; }

    public Result(bool success, ErrorResponse? error)
    {
        Success = success;
        Error = error;
    }

    public static Result Ok() => new(true, null);
    public static Result<T> Ok<T>(T value) => new(true, value, null);
    public static Result Failed(ErrorResponse err) => new(false, err);
    public static Result<T> Failed<T>(ErrorResponse err) => new(false, default!, err);

}

public class Result<T> : Result
{
    public T Value { get; }
    public Result(bool success, T value, ErrorResponse? error) : base(success, error) => Value = value;
}

