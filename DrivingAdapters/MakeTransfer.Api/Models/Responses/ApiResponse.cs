namespace MakeTransfer.Api.Models.Responses;

/// <summary>
/// Standard API response wrapper for consistent HTTP responses.
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }

    public static ApiResponse<T> SuccessResponse(
        T data, 
        string message = "Operation completed successfully", 
        string? correlationId = null)
    {
 return new ApiResponse<T>
        {
          Success = true,
            StatusCode = 200,
  Message = message,
     Data = data,
 CorrelationId = correlationId
    };
    }

    public static ApiResponse<T> ErrorResponse(
        int statusCode, 
        string message, 
 T? data = default, 
        string? correlationId = null)
    {
     return new ApiResponse<T>
        {
            Success = false,
     StatusCode = statusCode,
      Message = message,
            Data = data,
            CorrelationId = correlationId
        };
    }
}