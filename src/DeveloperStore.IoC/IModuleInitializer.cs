using Microsoft.AspNetCore.Builder;

namespace DeveloperStore.IoC;

/// <summary>
/// Interface for module initializers that configure dependency injection for specific layers.
/// </summary>
public interface IModuleInitializer
{
    void Initialize(WebApplicationBuilder builder);
}
