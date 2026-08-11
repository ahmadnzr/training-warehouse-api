using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;

namespace WarehouseWeb.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is AppException appException)
        {
            context.Response.StatusCode = appException.StatusCode;

            if (exception is ValidationException validationEx)
            {
                var validationResponse = new ApiValidationErrorResponse
                {
                    Errors = validationEx.Errors
                };
                await context.Response.WriteAsJsonAsync(validationResponse, SnakeCaseJsonOptions());
                return;
            }

            var errorResponse = new ApiErrorResponse
            {
                Message = appException.Message,
                Error = appException.Message
            };
            await context.Response.WriteAsJsonAsync(errorResponse, SnakeCaseJsonOptions());
            return;
        }

        context.Response.StatusCode = 500;
        var serverError = new ApiErrorResponse
        {
            Message = "Internal Server Error",
            Error = _environment.IsDevelopment() ? exception.Message : "Internal Server Error"
        };
        await context.Response.WriteAsJsonAsync(serverError, SnakeCaseJsonOptions());
    }

    private static JsonSerializerOptions SnakeCaseJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        return options;
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
