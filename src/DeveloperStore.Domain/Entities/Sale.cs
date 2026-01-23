using DeveloperStore.Domain.Common;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Domain.Entities;

/// <summary>
/// Represents a sale aggregate root in the system.
/// </summary>
public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = new();

    public string SaleNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public CustomerId Customer { get; set; } = null!;

    public BranchId Branch { get; set; } = null!;

    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public SaleStatus Status { get; set; }

    public decimal TotalAmount => CalculateTotalAmount();

    public bool IsCancelled => Status == SaleStatus.Cancelled;

    public void AddItem(SaleItem item)
    {
        if (IsCancelled)
        {
            throw new DomainException("Cannot add items to a cancelled sale.");
        }

        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.ApplyDiscountRules();

        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItem(Guid itemId, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
        {
            throw new DomainException("Cannot update items in a cancelled sale.");
        }

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            throw new DomainException($"Item with ID {itemId} not found in this sale.");
        }

        item.Quantity = quantity;
        item.UnitPrice = unitPrice;

        item.ApplyDiscountRules();

        item.UpdatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid itemId)
    {
        if (IsCancelled)
        {
            throw new DomainException("Cannot remove items from a cancelled sale.");
        }

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            throw new DomainException($"Item with ID {itemId} not found in this sale.");
        }

        _items.Remove(item);

        if (!_items.Any())
        {
            throw new DomainException("Sale must have at least one item. Cannot remove the last item.");
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelItem(Guid itemId)
    {
        if (IsCancelled)
        {
            throw new DomainException("Cannot cancel items in an already cancelled sale.");
        }

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            throw new DomainException($"Item with ID {itemId} not found in this sale.");
        }

        item.Cancel();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (IsCancelled)
        {
            throw new DomainException("Sale is already cancelled.");
        }

        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        foreach (var item in _items)
        {
            if (!item.IsCancelled)
            {
                item.Cancel();
            }
        }
    }

    private decimal CalculateTotalAmount()
    {
        return _items.Sum(item => item.TotalAmount);
    }

    public void InitializeItems(IEnumerable<SaleItem> items)
    {
        _items.Clear();
        _items.AddRange(items);

        if (!_items.Any())
        {
            throw new DomainException("Sale must have at least one item.");
        }
    }
}
