﻿using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;

public class RemoveProductCommandRequest : IRequest<RemoveProductCommandResponse>
{
    public string id { get; set; }
}
