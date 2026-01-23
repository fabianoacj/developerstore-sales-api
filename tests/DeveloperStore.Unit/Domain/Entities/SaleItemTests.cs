using Bogus;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;

namespace DeveloperStore.Unit.Domain.Entities;

public class SaleItemTests
{
    private readonly Faker _faker;

    public SaleItemTests()
    {
        _faker = new Faker();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void Quantity_ValidValue_ShouldSet(int quantity)
    {
        // Arrange
        var item = CreateSaleItem();

        // Act
        item.Quantity = quantity;

        // Assert
        item.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void Quantity_ZeroValue_ShouldThrowException()
    {
        // Arrange
        var item = CreateSaleItem();

        // Act
        Action act = () => item.Quantity = 0;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Fact]
    public void Quantity_NegativeValue_ShouldThrowException()
    {
        // Arrange
        var item = CreateSaleItem();

        // Act
        Action act = () => item.Quantity = -1;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Fact]
    public void Quantity_Above20_ShouldThrowException()
    {
        // Arrange
        var item = CreateSaleItem();

        // Act
        Action act = () => item.Quantity = 21;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("It's not possible to sell above 20 identical items.");
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    public void ApplyDiscountRules_QuantityBelow4_ShouldApplyNoDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = CreateSaleItem();
        item.Quantity = quantity;

        // Act
        item.ApplyDiscountRules();

        // Assert
        item.Discount.Should().Be(expectedDiscount);
    }

    [Theory]
    [InlineData(4, 10)]
    [InlineData(5, 10)]
    [InlineData(9, 10)]
    public void ApplyDiscountRules_Quantity4To9_ShouldApply10PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = CreateSaleItem();
        item.Quantity = quantity;

        // Act
        item.ApplyDiscountRules();

        // Assert
        item.Discount.Should().Be(expectedDiscount);
    }

    [Theory]
    [InlineData(10, 20)]
    [InlineData(15, 20)]
    [InlineData(20, 20)]
    public void ApplyDiscountRules_Quantity10To20_ShouldApply20PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = CreateSaleItem();
        item.Quantity = quantity;

        // Act
        item.ApplyDiscountRules();

        // Assert
        item.Discount.Should().Be(expectedDiscount);
    }

    [Fact]
    public void TotalAmount_WithNoDiscount_ShouldCalculateCorrectly()
    {
        // Arrange
        var item = CreateSaleItem();
        item.Quantity = 2;
        item.UnitPrice = 100m;
        item.ApplyDiscountRules();

        // Act
        var total = item.TotalAmount;

        // Assert
        total.Should().Be(200m); // 2 * 100 = 200
    }

    [Fact]
    public void TotalAmount_With10PercentDiscount_ShouldCalculateCorrectly()
    {
        // Arrange
        var item = CreateSaleItem();
        item.Quantity = 5;
        item.UnitPrice = 100m;
        item.ApplyDiscountRules();

        // Act
        var total = item.TotalAmount;

        // Assert
        total.Should().Be(450m); // (5 * 100) * 0.9 = 450
    }

    [Fact]
    public void TotalAmount_With20PercentDiscount_ShouldCalculateCorrectly()
    {
        // Arrange
        var item = CreateSaleItem();
        item.Quantity = 10;
        item.UnitPrice = 100m;
        item.ApplyDiscountRules();

        // Act
        var total = item.TotalAmount;

        // Assert
        total.Should().Be(800m); // (10 * 100) * 0.8 = 800
    }

    [Fact]
    public void Cancel_WhenCalled_ShouldSetIsCancelledToTrue()
    {
        // Arrange
        var item = CreateSaleItem();
        item.IsCancelled.Should().BeFalse();

        // Act
        item.Cancel();

        // Assert
        item.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public void Cancel_WhenCalled_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var item = CreateSaleItem();
        var originalUpdatedAt = item.UpdatedAt!.Value;
        Thread.Sleep(10); // Ensure time difference

        // Act
        item.Cancel();

        // Assert
        item.UpdatedAt.Should().NotBeNull();
        item.UpdatedAt!.Value.Should().BeAfter(originalUpdatedAt);
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
