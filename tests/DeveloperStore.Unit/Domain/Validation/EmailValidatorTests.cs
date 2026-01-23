using DeveloperStore.Domain.Validation;
using FluentValidation.TestHelper;

namespace DeveloperStore.Unit.Domain.Validation;

public class EmailValidatorTests
{
    private readonly EmailValidator _validator;

    public EmailValidatorTests()
    {
        _validator = new EmailValidator();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@company.co.uk")]
    [InlineData("first.last@subdomain.example.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("user_name@example.com")]
    [InlineData("123@example.com")]
    public void Validate_ValidEmail_ShouldPass(string email)
    {
        // Act
        var result = _validator.TestValidate(email);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyEmail_ShouldFail()
    {
        // Arrange
        var email = string.Empty;

        // Act
        var result = _validator.TestValidate(email);

        // Assert
        result.ShouldHaveValidationErrorFor(e => e)
            .WithErrorMessage("The email address cannot be empty.");
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user name@example.com")]
    [InlineData("user@example")]
    [InlineData("user@@example.com")]
    public void Validate_InvalidEmailFormat_ShouldFail(string email)
    {
        // Act
        var result = _validator.TestValidate(email);

        // Assert
        result.ShouldHaveValidationErrorFor(e => e)
            .WithErrorMessage("The provided email address is not valid.");
    }

    [Fact]
    public void Validate_EmailExceeds100Characters_ShouldFail()
    {
        // Arrange
        var email = new string('a', 90) + "@example.com"; // 103 characters

        // Act
        var result = _validator.TestValidate(email);

        // Assert
        result.ShouldHaveValidationErrorFor(e => e)
            .WithErrorMessage("The email address cannot be longer than 100 characters.");
    }

    [Fact]
    public void Validate_EmailExactly100Characters_ShouldPass()
    {
        // Arrange
        var email = new string('a', 87) + "@example.com"; // 100 characters

        // Act
        var result = _validator.TestValidate(email);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
