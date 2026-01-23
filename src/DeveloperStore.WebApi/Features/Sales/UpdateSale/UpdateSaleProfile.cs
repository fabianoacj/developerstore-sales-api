using AutoMapper;
using DeveloperStore.Application.Sales.UpdateSale;

namespace DeveloperStore.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// AutoMapper profile for UpdateSale request/response mapping.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes Update mappings.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemDto>();
    }
}
