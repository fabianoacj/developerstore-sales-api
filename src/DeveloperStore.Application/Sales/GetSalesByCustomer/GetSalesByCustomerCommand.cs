using MediatR;

namespace DeveloperStore.Application.Sales.GetSalesByCustomer;

/// <summary>
/// Command for retrieving sales by customer ID.
/// </summary>
public class GetSalesByCustomerCommand : IRequest<GetSalesByCustomerResult>
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSalesByCustomerCommand.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    public GetSalesByCustomerCommand(Guid customerId)
    {
        CustomerId = customerId;
    }
}

