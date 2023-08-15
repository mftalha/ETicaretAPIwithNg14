using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin;

public class FacebookLoginCommandReqeust : IRequest<FacebookLoginCommandResponse>
{
    public string AuthToken { get; set; }
    
}
