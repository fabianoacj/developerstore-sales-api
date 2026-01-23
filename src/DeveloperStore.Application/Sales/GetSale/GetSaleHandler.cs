using AutoMapper;
using MediatR;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Exceptions;

namespace DeveloperStore.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleCommand requests.
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSaleCommand request.
    /// </summary>
    /// <param name="command">The GetSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale details.</returns>
    public async Task<GetSaleResult> Handle(GetSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new NotFoundException("Sale", command.Id);

        var result = _mapper.Map<GetSaleResult>(sale);
        return result;
    }
}
