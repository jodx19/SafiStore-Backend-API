using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Filters
{
    /// <summary>
    /// Global result filter that wraps raw ObjectResult values in ApiResponse&lt;T&gt;,
    /// giving the frontend a consistent response contract — without stream buffering.
    ///
    /// This ONLY wraps ObjectResult (JSON data). It intentionally skips:
    ///   - FileResult (downloads, PDFs, images)
    ///   - Already-wrapped ApiResponse&lt;T&gt; (double-wrapping prevention)
    ///   - Redirect / Challenge / Forbid results
    ///   - 204 No Content
    /// </summary>
    public class ApiResponseResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Only process plain ObjectResult (includes OkObjectResult, BadRequestObjectResult, etc.)
            if (context.Result is not ObjectResult objectResult)
                return;

            // Skip if already an ApiResponse (prevents double-wrapping)
            var valueType = objectResult.Value?.GetType();
            if (valueType != null && IsApiResponse(valueType))
                return;

            // Skip null values (204 No Content pattern)
            if (objectResult.Value == null)
                return;

            // Wrap in ApiResponse<object> — preserves the original status code
            var isSuccess = objectResult.StatusCode is null or >= 200 and < 300;

            ApiResponse<object> wrapped;
            if (isSuccess)
            {
                wrapped = ApiResponse<object>.Ok(objectResult.Value);
            }
            else
            {
                // For error responses that aren't already ApiResponse, use generic error code
                wrapped = ApiResponse<object>.Error("REQUEST_FAILED", "An error occurred", objectResult.Value);
            }

            context.Result = new ObjectResult(wrapped)
            {
                StatusCode = objectResult.StatusCode
            };
        }

        public void OnResultExecuted(ResultExecutedContext context) { }

        private static bool IsApiResponse(Type type)
        {
            if (!type.IsGenericType) return false;
            var generic = type.GetGenericTypeDefinition();
            return generic == typeof(ApiResponse<>);
        }
    }
}
