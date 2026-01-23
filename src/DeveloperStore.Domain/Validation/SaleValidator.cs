using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Exceptions;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Input validator for Sale.
/// </summary>
public static class SaleValidator
{
    public static void Validate(Sale sale)
    {
        if (sale == null)
        {
            throw new ArgumentNullException(nameof(sale));
        }

        ValidateSaleNumber(sale);
        ValidateSaleDate(sale);
        ValidateCustomer(sale);
        ValidateBranch(sale);
        ValidateItems(sale);
    }

    private static void ValidateSaleNumber(Sale sale)
    {
        if (string.IsNullOrWhiteSpace(sale.SaleNumber))
        {
            throw new DomainException("Sale number is required.");
        }

        if (sale.SaleNumber.Length > 50)
        {
            throw new DomainException("Sale number cannot exceed 50 characters.");
        }
    }

    private static void ValidateSaleDate(Sale sale)
    {
        if (sale.SaleDate == default)
        {
            throw new DomainException("Sale date is required.");
        }

        if (sale.SaleDate > DateTime.UtcNow)
        {
            throw new DomainException("Sale date cannot be in the future.");
        }
    }

    private static void ValidateCustomer(Sale sale)
    {
        if (sale.Customer == null)
        {
            throw new DomainException("Customer is required.");
        }

        if (sale.Customer.Id == Guid.Empty)
        {
            throw new DomainException("Customer ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(sale.Customer.Name))
        {
            throw new DomainException("Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(sale.Customer.Email))
        {
            throw new DomainException("Customer email is required.");
        }
    }

    private static void ValidateBranch(Sale sale)
    {
        if (sale.Branch == null)
        {
            throw new DomainException("Branch is required.");
        }

        if (sale.Branch.Id == Guid.Empty)
        {
            throw new DomainException("Branch ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(sale.Branch.Name))
        {
            throw new DomainException("Branch name is required.");
        }
    }

    private static void ValidateItems(Sale sale)
    {
        if (sale.Items == null)
        {
            throw new DomainException("Items collection cannot be null.");
        }

        foreach (var item in sale.Items)
        {
            SaleItemValidator.Validate(item);
        }
    }
}
