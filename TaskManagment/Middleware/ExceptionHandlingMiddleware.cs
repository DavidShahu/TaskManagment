using System.Net;
using System.Text.Json;

namespace TaskManagment.Middleware;

public class ExceptionHandlingMiddleware
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
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            _logger.LogError(ex,
                "Unhandled exception. CorrelationId: {CorrelationId}",
                correlationId);
            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception ex,
        string? correlationId)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = ex switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.Conflict,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            Status = context.Response.StatusCode,
            Message = ex.Message,
            CorrelationId = correlationId
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}