using Bogus;
using DeveloperStore.Application.Events;
using DeveloperStore.Application.Sales.CancelSale;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Events;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace DeveloperStore.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly CancelSaleHandler _handler;
    private readonly Faker _faker;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CancelSaleHandler(_saleRepository, _eventPublisher);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCancelSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = CreateSale();
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        result.Message.Should().Contain("successfully cancelled");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallSaleCancel()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = CreateSale();
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateRepository()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = CreateSale();
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Any<Sale>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPublishSaleCancelledEvent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = CreateSale();
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received(1).PublishAsync(
            Arg.Any<SaleCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistentSale_ShouldThrowNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Sale with ID '{saleId}' was not found");
    }

    [Fact]
    public async Task Handle_AlreadyCancelledSale_ShouldThrowDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = CreateSale();
        sale.Status = SaleStatus.Cancelled;
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Sale is already cancelled.");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCancelAllItems()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = CreateSale();
        var item1 = CreateSaleItem();
        var item2 = CreateSaleItem();
        sale.AddItem(item1);
        sale.AddItem(item2);
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        item1.IsCancelled.Should().BeTrue();
        item2.IsCancelled.Should().BeTrue();
    }

    private Sale CreateSale()
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMdd}-{_faker.Random.Number(1, 99999):D5}",
            SaleDate = DateTime.UtcNow,
            Customer = new CustomerId(Guid.NewGuid(), _faker.Name.FullName(), _faker.Internet.Email()),
            Branch = new BranchId(Guid.NewGuid(), _faker.Company.CompanyName()),
            Status = SaleStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private SaleItem CreateSaleItem()
    {
        return new SaleItem
        {
            Id = Guid.NewGuid(),
            Product = new ProductId(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Commerce.Categories(1)[0]),
            Quantity = 5,
            UnitPrice = 100m,
            IsCancelled = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
