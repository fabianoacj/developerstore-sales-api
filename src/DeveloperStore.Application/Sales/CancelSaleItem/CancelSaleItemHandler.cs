using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.Events;
using DeveloperStore.Application.Events;

namespace DeveloperStore.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for processing CancelSaleItemCommand requests.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of CancelSaleItemHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    public CancelSaleItemHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the CancelSaleItemCommand request.
    /// </summary>
    /// <param name="command">The CancelSaleItem command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancellation result.</returns>
    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale == null)
            throw new NotFoundException("Sale", command.SaleId);

        var item = sale.Items.FirstOrDefault(i => i.Id == command.ItemId);
        if (item == null)
            throw new NotFoundException("Item", command.ItemId);

        sale.CancelItem(command.ItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish ItemCancelled event
        await _eventPublisher.PublishAsync(new SaleItemCancelledEvent(sale, item), cancellationToken);

        return new CancelSaleItemResult
        {
            SaleId = sale.Id,
            ItemId = command.ItemId,
            SaleNumber = sale.SaleNumber,
            Success = true,
            Message = $"Item {command.ItemId} in sale {sale.SaleNumber} has been successfully cancelled"
        };
    }
}
