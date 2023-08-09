using ETicaretAPI.Application.Repositories;
using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;

public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommandRequest, RemoveProductCommandResponse>
{
    readonly private IProductWriteRepository _productWriteRepository;
    public RemoveProductCommandHandler (IProductWriteRepository productWriteRepository)
    {
        _productWriteRepository = productWriteRepository;
    }
    public async Task<RemoveProductCommandResponse> Handle(RemoveProductCommandRequest request, CancellationToken cancellationToken)
    {
        await _productWriteRepository.RemoveAsync(request.id);
        await _productWriteRepository.SaveAsync();
        return new();
    }
}
