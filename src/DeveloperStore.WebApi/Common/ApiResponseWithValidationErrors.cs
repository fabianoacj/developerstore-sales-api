namespace DeveloperStore.WebApi.Common;

/// <summary>
/// API response model for validation errors.
/// </summary>
public class ApiResponseWithValidationErrors : ApiResponse
{
    /// <summary>
    /// Gets or sets the collection of validation errors.
    /// </summary>
    public IEnumerable<ValidationError> Errors { get; set; } = new List<ValidationError>();
}

/// <summary>
/// Represents a single validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the name of the property that failed validation.
    /// </summary>
    public string Property { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the validation error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
