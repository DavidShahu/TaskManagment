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
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, 404);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, ex, 403);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, 409);
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, ex, 400);
        }
        catch (Exception ex) // ← catches everything else including EF errors
        {
            _logger.LogError(ex,
                "Unhandled exception for {CorrelationId}",
                context.Items["CorrelationId"]);

            await HandleExceptionAsync(context,
                new Exception("An internal error occurred. Please try again."),
                500);
        }
    }


    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception ex,
        int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var correlationId = context.Items["CorrelationId"]?.ToString();

        var response = new
        {
            Status = statusCode,
            Message = ex.Message,
            CorrelationId = correlationId
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}