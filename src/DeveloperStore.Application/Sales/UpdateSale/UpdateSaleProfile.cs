using AutoMapper;
using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Application.Sales.UpdateSale;

/// <summary>
/// Profile for mapping between Sale entity and UpdateSale operations.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateSale operation.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<Sale, UpdateSaleResult>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count));
    }
}
