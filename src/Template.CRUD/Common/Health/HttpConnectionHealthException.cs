namespace Template.Api.Common.Health;

public class HttpConnectionHealthException : Exception
{
    public HttpResponseMessage? Response { get; }

    public HttpConnectionHealthException(string? message, HttpResponseMessage response) : this(message) => Response = response;

    private HttpConnectionHealthException(string? message) : base(message)
    {
    }
}
