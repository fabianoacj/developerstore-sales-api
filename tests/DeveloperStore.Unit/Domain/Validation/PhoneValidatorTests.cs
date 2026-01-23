using DeveloperStore.Domain.Validation;
using FluentValidation.TestHelper;

namespace DeveloperStore.Unit.Domain.Validation;

public class PhoneValidatorTests
{
    private readonly PhoneValidator _validator;

    public PhoneValidatorTests()
    {
        _validator = new PhoneValidator();
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("1234567890")]
    [InlineData("+1 (123) 456-7890")]
    [InlineData("123-456-7890")]
    [InlineData("(123) 456-7890")]
    public void Validate_ValidPhone_ShouldPass(string phone)
    {
        // Act
        var result = _validator.TestValidate(phone);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPhone_ShouldPass()
    {
        // Arrange
        var phone = string.Empty;

        // Act
        var result = _validator.TestValidate(phone);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PhoneExceeds20Characters_ShouldFail()
    {
        // Arrange
        var phone = new string('1', 21);

        // Act
        var result = _validator.TestValidate(phone);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p)
            .WithErrorMessage("Phone number must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_PhoneExactly20Characters_ShouldPass()
    {
        // Arrange
        var phone = new string('1', 20);

        // Act
        var result = _validator.TestValidate(phone);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
