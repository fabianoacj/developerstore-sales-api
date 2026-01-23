using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.WebApi.Common;

/// <summary>
/// Base controller with common API response methods.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    /// <summary>
    /// Returns a success response with data.
    /// </summary>
    protected IActionResult Ok<T>(T data) =>
        base.Ok(new ApiResponseWithData<T>
        {
            Data = data,
            Success = true,
            Message = "Operation completed successfully"
        });

    /// <summary>
    /// Returns a created response with data.
    /// </summary>
    protected IActionResult Created<T>(string routeName, object? routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T>
        {
            Data = data,
            Success = true,
            Message = "Resource created successfully"
        });

    /// <summary>
    /// Returns a bad request response with message.
    /// </summary>
    protected IActionResult BadRequest(string message) =>
        base.BadRequest(new ApiResponse
        {
            Message = message,
            Success = false
        });

    /// <summary>
    /// Returns a not found response.
    /// </summary>
    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse
        {
            Message = message,
            Success = false
        });
}

