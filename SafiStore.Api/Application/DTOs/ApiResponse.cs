using System.Text.Json.Serialization;

namespace SafiStore.Api.Application.DTOs
{
    /// <summary>
    /// Standardized API Response wrapper
    /// All API endpoints return this format for consistency
    /// </summary>
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        // Rename the CLR property to avoid collision with the static Error(...) method
        // Keep the JSON property name as "error" for consistency in API responses
        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorDetail? ErrorDetail { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Create success response
        /// </summary>
        public static ApiResponse<T> Ok(T? data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Operation successful"
            };
        }

        /// <summary>
        /// Create error response
        /// Note: the CLR property is named `ErrorDetail` to avoid a naming collision with this static method.
        /// The JSON key remains "error" via the JsonPropertyName attribute.
        /// </summary>
        public static ApiResponse<T> Error(string code, string message, object? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                ErrorDetail = new ErrorDetail
                {
                    Code = code,
                    Message = message,
                    Errors = errors
                }
            };
        }
    }

    /// <summary>
    /// Error detail structure
    /// </summary>
    public class ErrorDetail
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Errors { get; set; }
    }
}
