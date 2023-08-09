using MediatR;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.DeleteProductImage;

public class RemoveProductImageCommandRequest : IRequest<RemoveProductImageCommandResponse>
{
    public string id { get; set; }
    public string? imageId { get; set; }
}
