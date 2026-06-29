using NexusFlow.Application.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace NexusFlow.API.Middleware
{
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
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                KeyNotFoundException =>
                    ApiResponse<object>.Fail(exception.Message, 404),
                UnauthorizedAccessException =>
                    ApiResponse<object>.Fail(exception.Message, 401),
                ArgumentException =>
                    ApiResponse<object>.Fail(exception.Message, 400),
                _ =>
                    ApiResponse<object>.Fail("An unexpected error occurred.", 500)
            };

            context.Response.StatusCode = response.StatusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}