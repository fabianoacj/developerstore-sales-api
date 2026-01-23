namespace DeveloperStore.WebApi.Common;

/// <summary>
/// API response model with data payload.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponseWithData<T> : ApiResponse
{
    public T? Data { get; set; }
}
