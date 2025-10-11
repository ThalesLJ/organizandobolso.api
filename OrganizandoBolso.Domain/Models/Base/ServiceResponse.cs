using Swashbuckle.AspNetCore.Annotations;

namespace OrganizandoBolso.Domain.Models.Base;

[SwaggerSchema(Description = "Standard response for all operations")]
public class ServiceResponse<T>
{
    [SwaggerSchema(Description = "Indicates whether the operation was successful")]
    public bool Success { get; set; }

    [SwaggerSchema(Description = "Operation return message")]
    public string Message { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Data returned by the operation")]
    public T? Data { get; set; }

    [SwaggerSchema(Description = "List of encountered errors")]
    public List<string> Errors { get; set; } = new();

    [SwaggerSchema(Description = "Operation status code")]
    public int StatusCode { get; set; }

    public static ServiceResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
    {
        return new ServiceResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200
        };
    }

    public static ServiceResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }
}
