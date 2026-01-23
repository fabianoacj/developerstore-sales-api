using MediatR;

namespace DeveloperStore.Application.Events.GetSaleEvents;

/// <summary>
/// Query to retrieve all events for a specific sale.
/// </summary>
public class GetSaleEventsQuery : IRequest<GetSaleEventsResult>
{
    /// <summary>
    /// The ID of the sale to retrieve events for.
    /// </summary>
    public Guid SaleId { get; set; }

    public GetSaleEventsQuery(Guid saleId)
    {
        SaleId = saleId;
    }
}
