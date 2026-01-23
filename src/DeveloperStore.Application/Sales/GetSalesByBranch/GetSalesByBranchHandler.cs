using AutoMapper;
using FluentValidation;
using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Application.Sales.GetSale;

namespace DeveloperStore.Application.Sales.GetSalesByBranch;

/// <summary>
/// Handler for processing GetSalesByBranchCommand requests.
/// </summary>
public class GetSalesByBranchHandler : IRequestHandler<GetSalesByBranchCommand, GetSalesByBranchResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSalesByBranchHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetSalesByBranchHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSalesByBranchCommand request.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sales for the branch.</returns>
    public async Task<GetSalesByBranchResult> Handle(GetSalesByBranchCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetSalesByBranchValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sales = await _saleRepository.GetByBranchIdAsync(command.BranchId, cancellationToken);
        var salesList = sales.ToList();

        return new GetSalesByBranchResult
        {
            BranchId = command.BranchId,
            Sales = _mapper.Map<List<GetSaleResult>>(salesList),
            TotalCount = salesList.Count,
            TotalAmount = salesList.Sum(s => s.TotalAmount)
        };
    }
}
