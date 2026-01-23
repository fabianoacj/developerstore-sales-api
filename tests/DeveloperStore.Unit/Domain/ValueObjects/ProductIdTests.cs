using Bogus;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;

namespace DeveloperStore.Unit.Domain.ValueObjects;

public class ProductIdTests
{
    private readonly Faker _faker;

    public ProductIdTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_WithAllParameters_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = _faker.Commerce.ProductName();
        var category = _faker.Commerce.Categories(1)[0];
        var description = _faker.Commerce.ProductDescription();

        // Act
        var productId = ProductId.Create(id, title, category, description);

        // Assert
        productId.Should().NotBeNull();
        productId.Id.Should().Be(id);
        productId.Title.Should().Be(title);
        productId.Category.Should().Be(category);
        productId.Description.Should().Be(description);
    }

    [Fact]
    public void Create_WithoutDescription_ShouldCreateInstanceWithNullDescription()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = _faker.Commerce.ProductName();
        var category = _faker.Commerce.Categories(1)[0];

        // Act
        var productId = ProductId.Create(id, title, category);

        // Assert
        productId.Should().NotBeNull();
        productId.Id.Should().Be(id);
        productId.Title.Should().Be(title);
        productId.Category.Should().Be(category);
        productId.Description.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithAllParameters_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = _faker.Commerce.ProductName();
        var category = _faker.Commerce.Categories(1)[0];
        var description = _faker.Commerce.ProductDescription();

        // Act
        var productId = new ProductId(id, title, category, description);

        // Assert
        productId.Should().NotBeNull();
        productId.Id.Should().Be(id);
        productId.Title.Should().Be(title);
        productId.Category.Should().Be(category);
        productId.Description.Should().Be(description);
    }

    [Fact]
    public void Constructor_WithNullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = _faker.Commerce.Categories(1)[0];

        // Act
        Action act = () => new ProductId(id, null!, category);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Constructor_WithNullCategory_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = _faker.Commerce.ProductName();

        // Act
        Action act = () => new ProductId(id, title, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("category");
    }

    [Fact]
    public void Create_WithNullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = _faker.Commerce.Categories(1)[0];

        // Act
        Action act = () => ProductId.Create(id, null!, category);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Create_WithNullCategory_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = _faker.Commerce.ProductName();

        // Act
        Action act = () => ProductId.Create(id, title, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("category");
    }
}
