using AutoMapper;
using DeveloperStore.Application.Sales.GetSale;
using DeveloperStore.Domain.Common;

namespace DeveloperStore.Application.Sales.GetSales;

/// <summary>
/// AutoMapper profile for GetSales query.
/// </summary>
public class GetSalesProfile : Profile
{
    /// <summary>
    /// Initializes GetSales mappings.
    /// </summary>
    public GetSalesProfile()
    {
        CreateMap<PaginatedResult<Domain.Entities.Sale>, GetSalesResult>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.CurrentPage, opt => opt.MapFrom(src => src.CurrentPage))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));
    }
}
