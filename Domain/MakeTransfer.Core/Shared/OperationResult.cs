namespace MakeTransfer.Core.Shared;

/// <summary>
/// Generic result wrapper for operations that provides consistent success/failure handling
/// across all input ports.
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public sealed class OperationResult<T>
{
    public bool Success { get; }
    public int StatusCode { get; }
    public string Message { get; }
    public T? Data { get; }
    public DateTime Timestamp { get; }
    public string? CorrelationId { get; }

    private OperationResult(bool success, int statusCode, string message, T? data = default, string? correlationId = null)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
        Timestamp = DateTime.UtcNow;
        CorrelationId = correlationId;
    }

    public static OperationResult<T> SuccessResult(T data, string message = "Operation completed successfully", string? correlationId = null)
    {
        return new OperationResult<T>(true, 200, message, data, correlationId);
    }

    public static OperationResult<T> FailureResult(int statusCode, string message, string? correlationId = null)
    {
        return new OperationResult<T>(false, statusCode, message, default, correlationId);
    }

    public static OperationResult<T> NotFoundResult(string message = "Resource not found", string? correlationId = null)
    {
        return new OperationResult<T>(false, 404, message, default, correlationId);
    }

    public static OperationResult<T> ValidationErrorResult(string message, string? correlationId = null)
    {
        return new OperationResult<T>(false, 400, message, default, correlationId);
    }
}

/// <summary>
/// Non-generic operation result for operations that don't return data
/// </summary>
public sealed class OperationResult
{
    public bool Success { get; }
    public int StatusCode { get; }
    public string Message { get; }
    public DateTime Timestamp { get; }
    public string? CorrelationId { get; }

    private OperationResult(bool success, int statusCode, string message, string? correlationId = null)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Timestamp = DateTime.UtcNow;
        CorrelationId = correlationId;
    }

    public static OperationResult SuccessResult(string message = "Operation completed successfully", string? correlationId = null)
    {
        return new OperationResult(true, 200, message, correlationId);
    }

    public static OperationResult FailureResult(int statusCode, string message, string? correlationId = null)
    {
        return new OperationResult(false, statusCode, message, correlationId);
    }
}