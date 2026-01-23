using AutoMapper;
using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.Events;
using DeveloperStore.Application.Events;

namespace DeveloperStore.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request.
    /// </summary>
    /// <param name="command">The UpdateSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale details.</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new NotFoundException("Sale", command.Id);

        if (sale.IsCancelled)
            throw new DomainException("Cannot update a cancelled sale");

        // Update customer information
        sale.Customer = Domain.ValueObjects.CustomerId.Create(
            sale.Customer.Id,
            command.CustomerName,
            command.CustomerEmail,
            command.CustomerPhone);

        sale.UpdatedAt = DateTime.UtcNow;

        // Update existing items and track which item IDs are in the command
        var itemIdsInCommand = new HashSet<Guid>();
        foreach (var itemDto in command.Items.Where(i => i.Id != Guid.Empty))
        {
            itemIdsInCommand.Add(itemDto.Id);
            
            // Check if the item is already cancelled
            var existingItem = sale.Items.FirstOrDefault(i => i.Id == itemDto.Id);
            if (existingItem?.IsCancelled == true)
            {
                throw new DomainException($"Cannot update sale item {itemDto.Id} because it is already cancelled");
            }
            
            sale.UpdateItem(itemDto.Id, itemDto.Quantity, itemDto.UnitPrice);
        }

        // Remove items that are not in the command anymore
        var itemsToRemove = sale.Items
            .Where(i => !itemIdsInCommand.Contains(i.Id))
            .Select(i => i.Id)
            .ToList();

        foreach (var itemId in itemsToRemove)
        {
            sale.RemoveItem(itemId);
        }

        // Note: Adding new items is not supported in update - use CreateSale for new sales
        // If needed, this could be extended to support adding items

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish SaleModified event
        await _eventPublisher.PublishAsync(new SaleModifiedEvent(updatedSale), cancellationToken);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }
}
