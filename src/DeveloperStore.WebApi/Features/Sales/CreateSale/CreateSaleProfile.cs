using AutoMapper;
using DeveloperStore.Application.Sales.CreateSale;

namespace DeveloperStore.WebApi.Features.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for CreateSale request/response mapping.
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes Create mappings.
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemDto>();
        CreateMap<CreateSaleResult, CreateSaleResponse>();
        CreateMap<CreateSaleItemResult, CreateSaleItemResponse>();
    }
}
