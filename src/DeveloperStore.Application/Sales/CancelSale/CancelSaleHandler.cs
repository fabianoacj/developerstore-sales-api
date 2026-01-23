using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Exceptions;
using DeveloperStore.Domain.Events;
using DeveloperStore.Application.Events;

namespace DeveloperStore.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    public CancelSaleHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request.
    /// </summary>
    /// <param name="command">The CancelSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancellation result.</returns>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new NotFoundException("Sale", command.Id);

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish SaleCancelled event
        await _eventPublisher.PublishAsync(new SaleCancelledEvent(sale), cancellationToken);

        return new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Success = true,
            Message = $"Sale {sale.SaleNumber} has been successfully cancelled"
        };
    }
}
