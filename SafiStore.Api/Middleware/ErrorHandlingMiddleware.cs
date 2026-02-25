using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception");

                if (context.Response.HasStarted)
                {
                    // مهم جدًا: لو الريسبونس بدأ، سيبه يطلع
                    // response already started, cannot modify it. Re-throw to let server handle it.
                    throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string code = "INTERNAL_ERROR";
            string message = "An internal server error occurred.";

            switch (exception)
            {
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    code = "VALIDATION_ERROR";
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    code = "UNAUTHORIZED";
                    message = "Unauthorized access.";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    code = "NOT_FOUND";
                    message = "Resource not found.";
                    break;
            }

            var response = ApiResponse<object>.Error(code, message);
            var payload = JsonSerializer.Serialize(response);

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(payload);
        }
    }
}
