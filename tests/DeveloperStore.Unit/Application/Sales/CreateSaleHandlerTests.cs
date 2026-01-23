using AutoMapper;
using Bogus;
using DeveloperStore.Application.Events;
using DeveloperStore.Application.Sales.CreateSale;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Events;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace DeveloperStore.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly CreateSaleHandler _handler;
    private readonly Faker _faker;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _eventPublisher);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateSale()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };
        var expectedResult = CreateSaleResult();

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(expectedResult);
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallRepositoryCreateAsync()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _saleRepository.Received(1).CreateAsync(
            Arg.Any<Sale>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldGenerateUniqueSaleNumber()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.SaleNumber.Should().NotBeNullOrEmpty();
        sale.SaleNumber.Should().StartWith("SALE-");
        sale.SaleNumber.Should().MatchRegex(@"^SALE-\d{8}-\d{5}$");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetStatusToActive()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.Status.Should().Be(SaleStatus.Active);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAddItemsToSale()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem>
        {
            CreateSaleItem(),
            CreateSaleItem()
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.Items.Count.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPublishSaleCreatedEvent()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received(1).PublishAsync(
            Arg.Any<SaleCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingSaleNumber_ShouldIncrementSequence()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };
        var today = DateTime.UtcNow;
        var datePrefix = $"SALE-{today:yyyyMMdd}";
        var lastSaleNumber = $"{datePrefix}-00005";

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(datePrefix, Arg.Any<CancellationToken>())
            .Returns(lastSaleNumber);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.SaleNumber.Should().Be($"{datePrefix}-00006");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetSaleDateToUtcNow()
    {
        // Arrange
        var command = CreateValidCommand();
        var sale = CreateSale();
        var saleItems = new List<SaleItem> { CreateSaleItem() };
        var beforeExecution = DateTime.UtcNow;

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(saleItems);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(CreateSaleResult());
        _saleRepository.GetLastSaleNumberByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((string?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);
        var afterExecution = DateTime.UtcNow;

        // Assert
        sale.SaleDate.Should().BeOnOrAfter(beforeExecution);
        sale.SaleDate.Should().BeOnOrBefore(afterExecution);
    }

    private CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = _faker.Name.FullName(),
            CustomerEmail = _faker.Internet.Email(),
            CustomerPhone = _faker.Phone.PhoneNumber(),
            BranchId = Guid.NewGuid(),
            BranchName = _faker.Company.CompanyName(),
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductTitle = _faker.Commerce.ProductName(),
                    ProductCategory = _faker.Commerce.Categories(1)[0],
                    Quantity = 5,
                    UnitPrice = 100m
                }
            }
        };
    }

    private Sale CreateSale()
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            Customer = new CustomerId(Guid.NewGuid(), _faker.Name.FullName(), _faker.Internet.Email()),
            Branch = new BranchId(Guid.NewGuid(), _faker.Company.CompanyName()),
            Status = SaleStatus.Active
        };
    }

    private SaleItem CreateSaleItem()
    {
        return new SaleItem
        {
            Id = Guid.NewGuid(),
            Product = new ProductId(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Commerce.Categories(1)[0]),
            Quantity = 5,
            UnitPrice = 100m
        };
    }

    private CreateSaleResult CreateSaleResult()
    {
        return new CreateSaleResult
        {
            Id = Guid.NewGuid(),
            SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMdd}-00001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 500m,
            ItemCount = 1
        };
    }
}
