namespace WarehouseWeb.Api.Common;

public class ApiResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(string message, T? data)
    {
        Message = message;
        Data = data;
    }
}

public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
}

public class ApiValidationErrorResponse
{
    public string Message { get; set; } = "Validation Error";
    public IReadOnlyList<ValidationError>? Errors { get; set; }
}
