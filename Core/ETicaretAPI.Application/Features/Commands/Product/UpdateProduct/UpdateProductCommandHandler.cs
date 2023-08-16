using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
{
    readonly private IProductReadRepository _productReadRepository;
    readonly private IProductWriteRepository _productWriteRepository;
    readonly ILogger<UpdateProductCommandHandler> _logger; //loglama için.

    public UpdateProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, ILogger<UpdateProductCommandHandler> logger)
    {
        _productReadRepository = productReadRepository;
        _productWriteRepository = productWriteRepository;
        _logger = logger;
    }

    public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
    {
        ETicaretAPI.Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id);
        product.Stock = request.Stock;
        product.Name = request.Name;
        product.Price = request.Price;
        await _productWriteRepository.SaveAsync();
        _logger.LogInformation("Product güncellendi.."); //tabloya log'u atabilmek için

        return new();
    }
}
