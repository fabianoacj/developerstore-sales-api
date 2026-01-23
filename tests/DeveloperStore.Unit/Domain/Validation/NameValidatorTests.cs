using DeveloperStore.Domain.Validation;
using FluentValidation.TestHelper;

namespace DeveloperStore.Unit.Domain.Validation;

public class NameValidatorTests
{
    private readonly NameValidator _validator;

    public NameValidatorTests()
    {
        _validator = new NameValidator();
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData("Company Name")]
    [InlineData("A")]
    [InlineData("Branch-123")]
    public void Validate_ValidName_ShouldPass(string name)
    {
        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_ShouldFail()
    {
        // Arrange
        var name = string.Empty;

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldHaveValidationErrorFor(n => n)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Validate_NameExceeds200Characters_ShouldFail()
    {
        // Arrange
        var name = new string('A', 201);

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldHaveValidationErrorFor(n => n)
            .WithErrorMessage("Name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_NameExactly200Characters_ShouldPass()
    {
        // Arrange
        var name = new string('A', 200);

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
