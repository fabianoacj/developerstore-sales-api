using AutoMapper;
using DeveloperStore.Application.Sales.GetSale;

namespace DeveloperStore.WebApi.Features.Sales.GetSale;

/// <summary>
/// AutoMapper profile for GetSale response mapping.
/// </summary>
public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<GetSaleItemDto, GetSaleItemResponse>();
    }
}
