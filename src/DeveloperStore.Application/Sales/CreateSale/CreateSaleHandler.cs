using AutoMapper;
using MediatR;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.Events;
using DeveloperStore.Application.Events;

namespace DeveloperStore.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request.
    /// </summary>
    /// <param name="command">The CreateSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale details.</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        // Map command to Sale entity
        var sale = _mapper.Map<Sale>(command);
        
        // Auto-generate sale number and date
        sale.SaleNumber = await GenerateSaleNumberAsync(cancellationToken);
        sale.SaleDate = DateTime.UtcNow;
        sale.Status = SaleStatus.Active;
        sale.CreatedAt = DateTime.UtcNow;
        sale.UpdatedAt = DateTime.UtcNow;

        // Map and add items to the sale
        var items = _mapper.Map<List<SaleItem>>(command.Items);
        foreach (var item in items)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            sale.AddItem(item);
        }

        // Create the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Publish SaleCreated event
        await _eventPublisher.PublishAsync(new SaleCreatedEvent(createdSale), cancellationToken);

        // Map to result
        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }

    /// <summary>
    /// Generates a unique sale number.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A unique sale number in format SALE-YYYYMMDD-XXXXX.</returns>
    private async Task<string> GenerateSaleNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var datePrefix = $"SALE-{today:yyyyMMdd}";
        
        // Try to find the last sale number created today
        var lastSaleNumber = await _saleRepository.GetLastSaleNumberByPrefixAsync(datePrefix, cancellationToken);
        
        int nextSequence = 1;
        if (!string.IsNullOrEmpty(lastSaleNumber))
        {
            // Extract sequence number from last sale (e.g., SALE-20260123-00001 -> 00001)
            var parts = lastSaleNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastSequence))
            {
                nextSequence = lastSequence + 1;
            }
        }
        
        return $"{datePrefix}-{nextSequence:D5}";
    }
}
