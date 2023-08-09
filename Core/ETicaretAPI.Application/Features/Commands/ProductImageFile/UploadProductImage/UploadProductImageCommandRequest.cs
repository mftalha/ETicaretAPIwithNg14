using MediatR;
using Microsoft.AspNetCore.Http;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;

public class UploadProductImageCommandRequest : IRequest<UploadProductImageCommandResponse>
{
    public string id { get; set; }
    public IFormFileCollection? Files { get; set;}
}
