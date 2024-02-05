namespace Template.Api.Middleware;

public class FluentValidationExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public FluentValidationExceptionHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, FluentValidation.ValidationException ex)
    {
        var response = new
        {
            message = "Validation failed",
            errors = ex.Errors
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsync(jsonResponse);
    }
}
