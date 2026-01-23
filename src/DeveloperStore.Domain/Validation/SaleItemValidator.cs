using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Exceptions;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Input validator for SaleItem.
/// </summary>
public static class SaleItemValidator
{
    public static void Validate(SaleItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidateProduct(item);
        ValidateQuantity(item);
        ValidateUnitPrice(item);
        ValidateDiscount(item);
    }

    private static void ValidateProduct(SaleItem item)
    {
        if (item.Product == null)
        {
            throw new DomainException("Product is required for a sale item.");
        }

        if (item.Product.Id == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(item.Product.Title))
        {
            throw new DomainException("Product title is required.");
        }

        if (string.IsNullOrWhiteSpace(item.Product.Category))
        {
            throw new DomainException("Product category is required.");
        }
    }

    private static void ValidateQuantity(SaleItem item)
    {
        if (item.Quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }
    }

    private static void ValidateUnitPrice(SaleItem item)
    {
        if (item.UnitPrice <= 0)
        {
            throw new DomainException("Unit price must be greater than zero.");
        }
    }

    private static void ValidateDiscount(SaleItem item)
    {
        if (item.Discount < 0 || item.Discount > 100)
        {
            throw new DomainException("Discount must be between 0 and 100.");
        }
    }
}
