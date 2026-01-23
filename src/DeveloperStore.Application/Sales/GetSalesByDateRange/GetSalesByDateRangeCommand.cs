using MediatR;

namespace DeveloperStore.Application.Sales.GetSalesByDateRange;

/// <summary>
/// Command for retrieving sales within a date range.
/// </summary>
public class GetSalesByDateRangeCommand : IRequest<GetSalesByDateRangeResult>
{
    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSalesByDateRangeCommand.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    public GetSalesByDateRangeCommand(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}

