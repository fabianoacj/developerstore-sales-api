using Bogus;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;

namespace DeveloperStore.Unit.Domain.ValueObjects;

public class CustomerIdTests
{
    private readonly Faker _faker;

    public CustomerIdTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Name.FullName();
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var customerId = CustomerId.Create(id, name, email, phone);

        // Assert
        customerId.Should().NotBeNull();
        customerId.Id.Should().Be(id);
        customerId.Name.Should().Be(name);
        customerId.Email.Should().Be(email);
        customerId.Phone.Should().Be(phone);
    }

    [Fact]
    public void Create_WithoutPhone_ShouldCreateInstanceWithNullPhone()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Name.FullName();
        var email = _faker.Internet.Email();

        // Act
        var customerId = CustomerId.Create(id, name, email);

        // Assert
        customerId.Should().NotBeNull();
        customerId.Id.Should().Be(id);
        customerId.Name.Should().Be(name);
        customerId.Email.Should().Be(email);
        customerId.Phone.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Name.FullName();
        var email = _faker.Internet.Email();
        var phone = _faker.Phone.PhoneNumber();

        // Act
        var customerId = new CustomerId(id, name, email, phone);

        // Assert
        customerId.Should().NotBeNull();
        customerId.Id.Should().Be(id);
        customerId.Name.Should().Be(name);
        customerId.Email.Should().Be(email);
        customerId.Phone.Should().Be(phone);
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = _faker.Internet.Email();

        // Act
        Action act = () => new CustomerId(id, null!, email);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("name");
    }

    [Fact]
    public void Constructor_WithNullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Name.FullName();

        // Act
        Action act = () => new CustomerId(id, name, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("email");
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = _faker.Internet.Email();

        // Act
        Action act = () => CustomerId.Create(id, null!, email);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("name");
    }

    [Fact]
    public void Create_WithNullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = _faker.Name.FullName();

        // Act
        Action act = () => CustomerId.Create(id, name, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("email");
    }
}
