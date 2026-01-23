using FluentValidation;
using MediatR;
using DeveloperStore.Domain.Repositories;

namespace DeveloperStore.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    public CancelSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request.
    /// </summary>
    /// <param name="command">The CancelSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancellation result.</returns>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {command.Id} not found");

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Success = true,
            Message = $"Sale {sale.SaleNumber} has been successfully cancelled"
        };
    }
}
