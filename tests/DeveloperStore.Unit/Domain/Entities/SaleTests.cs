using Bogus;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;

namespace DeveloperStore.Unit.Domain.Entities;

public class SaleTests
{
    private readonly Faker _faker;

    public SaleTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void AddItem_ValidItem_ShouldAddToCollection()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();

        // Act
        sale.AddItem(item);

        // Assert
        sale.Items.Should().Contain(item);
        sale.Items.Count.Should().Be(1);
    }

    [Fact]
    public void AddItem_NullItem_ShouldThrowArgumentNullException()
    {
        // Arrange
        var sale = CreateSale();

        // Act
        Action act = () => sale.AddItem(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddItem_ToCancelledSale_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        sale.Status = SaleStatus.Cancelled;
        var item = CreateSaleItem();

        // Act
        Action act = () => sale.AddItem(item);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add items to a cancelled sale.");
    }

    [Fact]
    public void AddItem_ShouldApplyDiscountRules()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        item.Quantity = 5; // Should trigger 10% discount

        // Act
        sale.AddItem(item);

        // Assert
        item.Discount.Should().Be(10m);
    }

    [Fact]
    public void UpdateItem_ValidItem_ShouldUpdateQuantityAndPrice()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        sale.AddItem(item);

        // Act
        sale.UpdateItem(item.Id, 10, 150m);

        // Assert
        item.Quantity.Should().Be(10);
        item.UnitPrice.Should().Be(150m);
        item.Discount.Should().Be(20m); // 10 items = 20% discount
    }

    [Fact]
    public void UpdateItem_NonExistentItem_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var nonExistentId = Guid.NewGuid();

        // Act
        Action act = () => sale.UpdateItem(nonExistentId, 5, 100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Item with ID {nonExistentId} not found in this sale.");
    }

    [Fact]
    public void UpdateItem_OnCancelledSale_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        sale.AddItem(item);
        sale.Status = SaleStatus.Cancelled;

        // Act
        Action act = () => sale.UpdateItem(item.Id, 5, 100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot update items in a cancelled sale.");
    }

    [Fact]
    public void RemoveItem_ValidItem_ShouldRemoveFromCollection()
    {
        // Arrange
        var sale = CreateSale();
        var item1 = CreateSaleItem();
        var item2 = CreateSaleItem();
        sale.AddItem(item1);
        sale.AddItem(item2);

        // Act
        sale.RemoveItem(item1.Id);

        // Assert
        sale.Items.Should().NotContain(item1);
        sale.Items.Should().Contain(item2);
        sale.Items.Count.Should().Be(1);
    }

    [Fact]
    public void RemoveItem_LastItem_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        sale.AddItem(item);

        // Act
        Action act = () => sale.RemoveItem(item.Id);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Sale must have at least one item. Cannot remove the last item.");
    }

    [Fact]
    public void RemoveItem_FromCancelledSale_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var item1 = CreateSaleItem();
        var item2 = CreateSaleItem();
        sale.AddItem(item1);
        sale.AddItem(item2);
        sale.Status = SaleStatus.Cancelled;

        // Act
        Action act = () => sale.RemoveItem(item1.Id);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot remove items from a cancelled sale.");
    }

    [Fact]
    public void CancelItem_ValidItem_ShouldCancelItem()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        sale.AddItem(item);

        // Act
        sale.CancelItem(item.Id);

        // Assert
        item.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public void CancelItem_AlreadyCancelledItem_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        sale.AddItem(item);
        item.Cancel();

        // Act
        Action act = () => sale.CancelItem(item.Id);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Item with ID {item.Id} is already cancelled.");
    }

    [Fact]
    public void CancelItem_OnCancelledSale_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var item = CreateSaleItem();
        sale.AddItem(item);
        sale.Status = SaleStatus.Cancelled;

        // Act
        Action act = () => sale.CancelItem(item.Id);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel items in an already cancelled sale.");
    }

    [Fact]
    public void Cancel_ActiveSale_ShouldCancelSaleAndAllItems()
    {
        // Arrange
        var sale = CreateSale();
        var item1 = CreateSaleItem();
        var item2 = CreateSaleItem();
        sale.AddItem(item1);
        sale.AddItem(item2);

        // Act
        sale.Cancel();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.IsCancelled.Should().BeTrue();
        item1.IsCancelled.Should().BeTrue();
        item2.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public void Cancel_AlreadyCancelledSale_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        sale.Status = SaleStatus.Cancelled;

        // Act
        Action act = () => sale.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Sale is already cancelled.");
    }

    [Fact]
    public void TotalAmount_ShouldSumAllItemTotals()
    {
        // Arrange
        var sale = CreateSale();
        var item1 = CreateSaleItem();
        item1.Quantity = 2;
        item1.UnitPrice = 100m;
        item1.ApplyDiscountRules(); // No discount

        var item2 = CreateSaleItem();
        item2.Quantity = 10;
        item2.UnitPrice = 50m;
        item2.ApplyDiscountRules(); // 20% discount

        sale.AddItem(item1);
        sale.AddItem(item2);

        // Act
        var total = sale.TotalAmount;

        // Assert
        // Item1: 2 * 100 = 200
        // Item2: 10 * 50 * 0.8 = 400
        // Total: 600
        total.Should().Be(600m);
    }

    [Fact]
    public void InitializeItems_ValidItems_ShouldSetItems()
    {
        // Arrange
        var sale = CreateSale();
        var items = new List<SaleItem>
        {
            CreateSaleItem(),
            CreateSaleItem()
        };

        // Act
        sale.InitializeItems(items);

        // Assert
        sale.Items.Count.Should().Be(2);
    }

    [Fact]
    public void InitializeItems_EmptyList_ShouldThrowDomainException()
    {
        // Arrange
        var sale = CreateSale();
        var items = new List<SaleItem>();

        // Act
        Action act = () => sale.InitializeItems(items);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Sale must have at least one item.");
    }

    private Sale CreateSale()
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMdd}-{_faker.Random.Number(1, 99999):D5}",
            SaleDate = DateTime.UtcNow,
            Customer = new CustomerId(Guid.NewGuid(), _faker.Name.FullName(), _faker.Internet.Email(), _faker.Phone.PhoneNumber()),
            Branch = new BranchId(Guid.NewGuid(), _faker.Company.CompanyName()),
            Status = SaleStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private SaleItem CreateSaleItem()
    {
        return new SaleItem
        {
            Id = Guid.NewGuid(),
            Product = new ProductId(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Commerce.Categories(1)[0]),
            Quantity = 1,
            UnitPrice = _faker.Random.Decimal(10, 1000),
            IsCancelled = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
