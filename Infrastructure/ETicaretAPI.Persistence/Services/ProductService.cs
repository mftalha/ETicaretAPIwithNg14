using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using System.Text.Json;

namespace ETicaretAPI.Persistence.Services;

public class ProductService : IProductService
{
	readonly IProductReadRepository _productReadRepository;
	readonly IQRCodeService _qrCodeService;

	public ProductService(IProductReadRepository productReadRepository, IQRCodeService qrCodeService)
	{
		_productReadRepository = productReadRepository;
		_qrCodeService = qrCodeService;
	}

	public async Task<byte[]> QRCodeToProductAsync(string productId)
	{
		Product product = await _productReadRepository.GetByIdAsync(productId);
		if (product == null)
			throw new Exception("Product not found");

		var plainObject = new
		{
			product.Id,
			product.Name,
			product.Price,
			product.Stock,
			product.CreatedDate
		};

		string plainText = JsonSerializer.Serialize(plainObject);

		return _qrCodeService.GenerateQRCode(plainText);
	}
}
