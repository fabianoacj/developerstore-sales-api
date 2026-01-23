using AutoMapper;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Application.Sales.CreateSale;

/// <summary>
/// Profile for mapping between Sale entity and CreateSale operations.
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateSale operation.
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src =>
                CustomerId.Create(src.CustomerId, src.CustomerName, src.CustomerEmail, src.CustomerPhone)))
            .ForMember(dest => dest.Branch, opt => opt.MapFrom(src =>
                BranchId.Create(src.BranchId, src.BranchName)))
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<CreateSaleItemDto, SaleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src =>
                ProductId.Create(src.ProductId, src.ProductTitle, src.ProductCategory, src.ProductDescription)))
            .ForMember(dest => dest.Discount, opt => opt.Ignore())
            .ForMember(dest => dest.IsCancelled, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Sale, CreateSaleResult>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItem, CreateSaleItemResult>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Product.Title));
    }
}
