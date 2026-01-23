using AutoMapper;
using FluentValidation;
using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Application.Sales.GetSale;

namespace DeveloperStore.Application.Sales.GetSalesByCustomer;

/// <summary>
/// Handler for processing GetSalesByCustomerCommand requests.
/// </summary>
public class GetSalesByCustomerHandler : IRequestHandler<GetSalesByCustomerCommand, GetSalesByCustomerResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSalesByCustomerHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetSalesByCustomerHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSalesByCustomerCommand request.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sales for the customer.</returns>
    public async Task<GetSalesByCustomerResult> Handle(GetSalesByCustomerCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetSalesByCustomerValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sales = await _saleRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);
        var salesList = sales.ToList();

        return new GetSalesByCustomerResult
        {
            CustomerId = command.CustomerId,
            Sales = _mapper.Map<List<GetSaleResult>>(salesList),
            TotalCount = salesList.Count,
            TotalAmount = salesList.Sum(s => s.TotalAmount)
        };
    }
}
