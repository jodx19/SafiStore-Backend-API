namespace SafiStore.Api.Application.DTOs
{
    // Generic service-level result used to avoid throwing for expected failures.
    // Services return this object to indicate success or a known failure (e.g. duplicate key).
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }

        // Success factory to return a successful result with data
        public static ServiceResult<T> SuccessResult(T data, string? message = null)
        {
            return new ServiceResult<T> { Success = true, Data = data, Message = message };
        }

        // Fail factory to return a failure with a machine-friendly error code and human message
        public static ServiceResult<T> Fail(string errorCode, string message)
        {
            return new ServiceResult<T> { Success = false, ErrorCode = errorCode, Message = message };
        }
    }
}
