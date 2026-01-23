using MediatR;

namespace DeveloperStore.Application.Sales.GetSale;

/// <summary>
/// Command for retrieving a sale by its unique identifier.
/// </summary>
public class GetSaleCommand : IRequest<GetSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSaleCommand.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    public GetSaleCommand(Guid id)
    {
        Id = id;
    }
}
