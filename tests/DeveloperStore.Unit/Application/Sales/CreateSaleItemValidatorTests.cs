using Bogus;
using DeveloperStore.Application.Sales.CreateSale;
using FluentValidation.TestHelper;
using FluentAssertions;

namespace DeveloperStore.Unit.Application.Sales;

public class CreateSaleItemValidatorTests
{
    private readonly CreateSaleItemValidator _validator;
    private readonly Faker _faker;

    public CreateSaleItemValidatorTests()
    {
        _validator = new CreateSaleItemValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Validate_ValidItem_ShouldPass()
    {
        // Arrange
        var item = CreateValidItem();

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyProductId_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductId)
            .WithErrorMessage("Product ID is required");
    }

    [Fact]
    public void Validate_EmptyProductTitle_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductTitle = string.Empty;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductTitle)
            .WithErrorMessage("Product title is required.");
    }

    [Fact]
    public void Validate_ProductTitleExceeds200Characters_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductTitle = new string('A', 201);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductTitle)
            .WithErrorMessage("Product title must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_EmptyProductCategory_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductCategory = string.Empty;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductCategory)
            .WithErrorMessage("Product category is required.");
    }

    [Fact]
    public void Validate_ProductCategoryExceeds100Characters_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductCategory = new string('A', 101);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductCategory)
            .WithErrorMessage("Product category must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ProductDescriptionExceeds500Characters_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductDescription = new string('A', 501);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductDescription)
            .WithErrorMessage("Product description must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_NullProductDescription_ShouldPass()
    {
        // Arrange
        var item = CreateValidItem();
        item.ProductDescription = null;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProductDescription);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidQuantity_ShouldFail(int quantity)
    {
        // Arrange
        var item = CreateValidItem();
        item.Quantity = quantity;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be greater than zero");
    }

    [Fact]
    public void Validate_QuantityAbove20_ShouldFail()
    {
        // Arrange
        var item = CreateValidItem();
        item.Quantity = 21;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("It's not possible to sell above 20 identical items");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void Validate_ValidQuantity_ShouldPass(int quantity)
    {
        // Arrange
        var item = CreateValidItem();
        item.Quantity = quantity;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidUnitPrice_ShouldFail(decimal unitPrice)
    {
        // Arrange
        var item = CreateValidItem();
        item.UnitPrice = unitPrice;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
            .WithErrorMessage("Unit price must be greater than zero");
    }

    [Fact]
    public void Validate_ValidUnitPrice_ShouldPass()
    {
        // Arrange
        var item = CreateValidItem();
        item.UnitPrice = 100.50m;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
    }

    private CreateSaleItemDto CreateValidItem()
    {
        return new CreateSaleItemDto
        {
            ProductId = Guid.NewGuid(),
            ProductTitle = _faker.Commerce.ProductName(),
            ProductCategory = _faker.Commerce.Categories(1)[0],
            ProductDescription = _faker.Commerce.ProductDescription(),
            Quantity = 5,
            UnitPrice = _faker.Random.Decimal(10, 1000)
        };
    }
}
