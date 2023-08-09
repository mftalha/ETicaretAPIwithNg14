using ETicaretAPI.Application.Abstractions.Stroge;
using ETicaretAPI.Application.Repositories;
using MediatR;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
{
    readonly private IStorageService _storageService;
    readonly private IProductReadRepository _productReadRepository;
    readonly private IProductImageFileWriteRepository _productImageFileWriteRepository;

    public UploadProductImageCommandHandler(IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IStorageService storageService)
    {
        _productReadRepository = productReadRepository;
        _productImageFileWriteRepository = productImageFileWriteRepository;
        _storageService = storageService;
    }

    public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
    {
        List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", request.Files);

        Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.id);

        /* : resim eklemede bunu da kullanabiliyoruz aşşağıdaki yapıya alternatif
        foreach(var r in result)
        {
            product.ProductImageFiles.Add(new()
            {
                FileName = r.fileName,
                Path = r.pathOrContainerName,
                Storage = _storageService.StrogeName,
                Products = new List<Product>() { product}
            });
        }
        */

        await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new Domain.Entities.ProductImageFile
        {
            FileName = r.fileName,
            Path = r.pathOrContainerName,
            Storage = _storageService.StrogeName,
            Products = new List<Domain.Entities.Product>() { product }
        }).ToList());

        await _productImageFileWriteRepository.SaveAsync();

        return new();
    }
}
