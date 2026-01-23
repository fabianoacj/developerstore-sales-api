using AutoMapper;
using FluentValidation;
using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Application.Sales.GetSale;

namespace DeveloperStore.Application.Sales.GetSalesByDateRange;

/// <summary>
/// Handler for processing GetSalesByDateRangeCommand requests.
/// </summary>
public class GetSalesByDateRangeHandler : IRequestHandler<GetSalesByDateRangeCommand, GetSalesByDateRangeResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSalesByDateRangeHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetSalesByDateRangeHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSalesByDateRangeCommand request.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sales within the date range.</returns>
    public async Task<GetSalesByDateRangeResult> Handle(GetSalesByDateRangeCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetSalesByDateRangeValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sales = await _saleRepository.GetByDateRangeAsync(command.StartDate, command.EndDate, cancellationToken);
        var salesList = sales.ToList();

        return new GetSalesByDateRangeResult
        {
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Sales = _mapper.Map<List<GetSaleResult>>(salesList),
            TotalCount = salesList.Count,
            TotalAmount = salesList.Sum(s => s.TotalAmount)
        };
    }
}
