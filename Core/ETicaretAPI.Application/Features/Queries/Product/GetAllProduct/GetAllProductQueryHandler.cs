using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
{
    readonly private IProductReadRepository _productReadRepository;
    readonly ILogger<GetAllProductQueryHandler> _logger;
    public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<GetAllProductQueryHandler> logger)
    {
        _productReadRepository = productReadRepository;
        _logger = logger;
    }

    public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all product");
        var totalCount = _productReadRepository.GetAll(false).Count();
        //skip ile kaçıncı veriye kadar alınacağını söylüyorum mesela 15 ; take ile de 5 tane alınacaksa = 10 - 15 arası veriler getiriliyor gibi bi mantık anlıyorum.
        var products = _productReadRepository.GetAll(false).Skip(request.Page * request.Size).Take(request.Size).Select(p => new
        { // bu method çağrıldığında bütün tablo verilerini değilde sadece bu verileri döndür diyoruz.
            p.Id,
            p.Name,
            p.Stock,
            p.Price,
            p.CreatedDate,
            p.UpdatedDate
        }).ToList(); // 5(sayfada gösterim adedi) *3(kaçıncı sayfa) = 15 veriyi getir ;; Skip(pagination.Size) == bu kadar veriyi getir gibi mantık galiba.

        return new()
        {
            Products = products,
            TotalCount = totalCount
        };
    }
}
