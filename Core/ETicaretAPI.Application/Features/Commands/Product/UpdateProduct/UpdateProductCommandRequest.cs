using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;

public class UpdateProductCommandRequest : IRequest<UpdateProductCommandResponse>
{
    public string Id { get; set; } //id'yi paremetre olarakda alabiliriz. ama böyle gövdede almak daha doğruymuş.
    public string Name { get; set; }
    public int Stock { get; set; }
    public float Price { get; set; }
}
