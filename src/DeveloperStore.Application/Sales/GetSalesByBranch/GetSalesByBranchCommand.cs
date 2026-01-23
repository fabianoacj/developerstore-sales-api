using MediatR;

namespace DeveloperStore.Application.Sales.GetSalesByBranch;

/// <summary>
/// Command for retrieving sales by branch ID.
/// </summary>
public class GetSalesByBranchCommand : IRequest<GetSalesByBranchResult>
{
    /// <summary>
    /// Gets or sets the branch ID.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSalesByBranchCommand.
    /// </summary>
    /// <param name="branchId">The branch ID.</param>
    public GetSalesByBranchCommand(Guid branchId)
    {
        BranchId = branchId;
    }
}
