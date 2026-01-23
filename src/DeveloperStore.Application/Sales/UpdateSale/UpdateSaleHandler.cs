using AutoMapper;
using FluentValidation;
using MediatR;
using DeveloperStore.Domain.Repositories;

namespace DeveloperStore.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request.
    /// </summary>
    /// <param name="command">The UpdateSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale details.</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {command.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        // Update sale date
        sale.SaleDate = command.SaleDate;
        sale.UpdatedAt = DateTime.UtcNow;

        // Update existing items and track which ones are in the command
        var existingItemIds = new HashSet<Guid>();
        foreach (var itemDto in command.Items.Where(i => i.Id != Guid.Empty))
        {
            existingItemIds.Add(itemDto.Id);
            sale.UpdateItem(itemDto.Id, itemDto.Quantity, itemDto.UnitPrice);
        }

        // Remove items that are not in the command anymore
        var itemsToRemove = sale.Items
            .Where(i => !existingItemIds.Contains(i.Id))
            .Select(i => i.Id)
            .ToList();

        foreach (var itemId in itemsToRemove)
        {
            sale.RemoveItem(itemId);
        }

        // Note: Adding new items is not supported in update - use CreateSale for new sales
        // If needed, this could be extended to support adding items

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }
}
