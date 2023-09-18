namespace ETicaretAPI.Application.Abstractions.Services;

public interface IProductService
{
	Task<byte[]> QRCodeToProductAsync(string productId);
}
