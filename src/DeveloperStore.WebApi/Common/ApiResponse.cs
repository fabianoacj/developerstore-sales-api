namespace DeveloperStore.WebApi.Common;

/// <summary>
/// Base API response model.
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}
