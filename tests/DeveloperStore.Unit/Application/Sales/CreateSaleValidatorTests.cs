using Bogus;
using DeveloperStore.Application.Sales.CreateSale;
using FluentValidation.TestHelper;
using FluentAssertions;

namespace DeveloperStore.Unit.Application.Sales;

public class CreateSaleValidatorTests
{
    private readonly CreateSaleValidator _validator;
    private readonly Faker _faker;

    public CreateSaleValidatorTests()
    {
        _validator = new CreateSaleValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyCustomerId_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerId)
            .WithErrorMessage("Customer ID is required");
    }

    [Fact]
    public void Validate_EmptyCustomerName_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerName = string.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void Validate_CustomerNameExceeds200Characters_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerName = new string('A', 201);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void Validate_InvalidCustomerEmail_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerEmail = "invalid-email";

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }

    [Fact]
    public void Validate_CustomerPhoneExceeds20Characters_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerPhone = new string('1', 21);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerPhone);
    }

    [Fact]
    public void Validate_NullCustomerPhone_ShouldPass()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CustomerPhone = null;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CustomerPhone);
    }

    [Fact]
    public void Validate_EmptyBranchId_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.BranchId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchId)
            .WithErrorMessage("Branch ID is required");
    }

    [Fact]
    public void Validate_EmptyBranchName_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.BranchName = string.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchName);
    }

    [Fact]
    public void Validate_EmptyItems_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items = new List<CreateSaleItemDto>();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("Sale must have at least one item");
    }

    [Fact]
    public void Validate_NullItems_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items = null!;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    private CreateSaleCommand CreateValidCommand()
    {
        var phoneNumber = _faker.Phone.PhoneNumber();
        return new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = _faker.Name.FullName(),
            CustomerEmail = _faker.Internet.Email(),
            CustomerPhone = phoneNumber.Length > 15 ? phoneNumber.Substring(0, 15) : phoneNumber,
            BranchId = Guid.NewGuid(),
            BranchName = _faker.Company.CompanyName(),
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductTitle = _faker.Commerce.ProductName(),
                    ProductCategory = _faker.Commerce.Categories(1)[0],
                    ProductDescription = _faker.Commerce.ProductDescription(),
                    Quantity = 5,
                    UnitPrice = _faker.Random.Decimal(10, 1000)
                }
            }
        };
    }
}
