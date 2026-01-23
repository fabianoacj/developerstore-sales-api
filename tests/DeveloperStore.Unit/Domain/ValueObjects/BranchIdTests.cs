using Bogus;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;

namespace DeveloperStore.Unit.Domain.ValueObjects;

public class BranchIdTests
{
    private readonly Faker _faker;

    public BranchIdTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Company.CompanyName();

        // Act
        var branchId = BranchId.Create(id, name);

        // Assert
        branchId.Should().NotBeNull();
        branchId.Id.Should().Be(id);
        branchId.Name.Should().Be(name);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Company.CompanyName();

        // Act
        var branchId = new BranchId(id, name);

        // Assert
        branchId.Should().NotBeNull();
        branchId.Id.Should().Be(id);
        branchId.Name.Should().Be(name);
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => new BranchId(id, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("name");
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => BranchId.Create(id, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("name");
    }
}
