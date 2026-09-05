using System.Net;
using System.Text.Json;

namespace OmniPay.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, level) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, LogLevel.Warning),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message, LogLevel.Warning),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message, LogLevel.Warning),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", LogLevel.Error)
        };

        _logger.Log(
            level,
            exception,
            "Request {Method} {Path} failed with status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            (int)statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(payload);
    }
}
