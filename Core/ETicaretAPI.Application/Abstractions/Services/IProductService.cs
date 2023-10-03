namespace ETicaretAPI.Application.Abstractions.Services;

public interface IProductService
{
	Task<byte[]> QRCodeToProductAsync(string productId);
	Task StockUpdateToProductAsync(string productId, int stock);
}
