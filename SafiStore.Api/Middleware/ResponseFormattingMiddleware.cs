// Middleware/ResponseFormattingMiddleware.cs
using Microsoft.AspNetCore.Http;
using SafiStore.Api.Application.DTOs;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ResponseFormattingMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseFormattingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        // Only attempt to wrap JSON responses for API endpoints
        var path = context.Request.Path.Value ?? string.Empty;
        var shouldSkip = path.StartsWith("/swagger") || path.StartsWith("/health") || path.StartsWith("/metrics") || path.StartsWith("/static");
        if (shouldSkip)
        {
            await _next(context);
            return;
        }

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);

            // rewind & read
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            // restore original stream
            context.Response.Body = originalBodyStream;

            // If response already started, give up on wrapping to avoid corrupting stream
            if (context.Response.HasStarted)
            {
                return;
            }

            // Only wrap if content-type is application/json or empty (some frameworks omit it)
            var contentType = context.Response.ContentType ?? string.Empty;
            if (!contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(responseBody))
            {
                // write original payload back
                await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(responseBody));
                return;
            }

            // If empty response body (e.g., 204), just set appropriate status and return
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                // nothing to wrap
                return;
            }

            // Try to detect if already wrapped (simple check for "success" and "data" keys)
            var alreadyWrapped = responseBody.Contains("\"success\"", StringComparison.OrdinalIgnoreCase) && responseBody.Contains("\"data\"", StringComparison.OrdinalIgnoreCase);
            if (alreadyWrapped)
            {
                await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(responseBody));
                return;
            }

            object? parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<object>(responseBody);
            }
            catch
            {
                parsed = responseBody;
            }

            var wrapper = ApiResponse<object>.Ok(parsed);
            var payload = JsonSerializer.Serialize(wrapper);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(payload);
        }
        finally
        {
            // ensure body restored even on exception
            context.Response.Body = originalBodyStream;
        }
    }
}
