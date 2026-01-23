using DeveloperStore.Domain.Common;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Domain.Entities;

/// <summary>
/// Represents an item in a sale with quantity, pricing, and discount information.
/// </summary>
public class SaleItem : BaseEntity
{
    private int _quantity;
    private decimal _discount;

    public ProductId Product { get; set; } = null!;

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value > 20)
            {
                throw new DomainException("It's not possible to sell above 20 identical items.");
            }

            if (value <= 0)
            {
                throw new DomainException("Quantity must be greater than zero.");
            }

            _quantity = value;
        }
    }

    public decimal UnitPrice { get; set; }

    public decimal Discount
    {
        get => _discount;
        private set
        {
            ValidateDiscount(value);
            _discount = value;
        }
    }

    public bool IsCancelled { get; set; }

    public decimal TotalAmount => CalculateTotalAmount();

    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Applies the appropriate discount based on quantity.
    /// Business Rules:
    /// - 4-9 items: 10% discount
    /// - 10-20 items: 20% discount
    /// - Below 4 items: No discount
    /// - Above 20 items: Not allowed (enforced by Quantity setter)
    /// </summary>
    public void ApplyDiscountRules()
    {
        if (Quantity >= 10 && Quantity <= 20)
        {
            Discount = 20m;
        }
        else if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 10m;
        }
        else if (Quantity < 4)
        {
            Discount = 0m;
        }
    }

    /// <summary>
    /// Validates discount business.
    /// Business Rules:
    /// - Purchases below 4 items cannot have a discount
    /// - Quantities 4-9: max 10% discount
    /// - Quantities 10-20: max 20% discount
    /// </summary>
    private void ValidateDiscount(decimal discount)
    {
        if (discount < 0 || discount > 100)
        {
            throw new DomainException("Discount must be between 0 and 100.");
        }

        if (Quantity < 4 && discount > 0)
        {
            throw new DomainException("Purchases below 4 items cannot have a discount.");
        }

        if (Quantity >= 4 && Quantity < 10 && discount > 10m)
        {
            throw new DomainException("Discount cannot exceed 10% for quantities between 4 and 9 items.");
        }

        if (Quantity >= 10 && Quantity <= 20 && discount > 20m)
        {
            throw new DomainException("Discount cannot exceed 20% for quantities between 10 and 20 items.");
        }
    }

    /// <summary>
    /// Calculates the total amount for this item.
    /// Formula: (UnitPrice * Quantity) * (1 - Discount/100)
    /// </summary>
    private decimal CalculateTotalAmount()
    {
        if (IsCancelled)
        {
            return 0m;
        }

        var subtotal = UnitPrice * Quantity;
        var discountAmount = subtotal * (Discount / 100m);
        return subtotal - discountAmount;
    }
}
