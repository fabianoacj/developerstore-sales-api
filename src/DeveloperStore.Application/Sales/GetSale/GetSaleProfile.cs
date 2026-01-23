using AutoMapper;
using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Application.Sales.GetSale;

/// <summary>
/// Profile for mapping between Sale entity and GetSale operations.
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale operation.
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<Sale, GetSaleResult>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.Phone))
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Branch.Id))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItem, GetSaleItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Product.Title))
            .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Product.Category))
            .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));
    }
}
