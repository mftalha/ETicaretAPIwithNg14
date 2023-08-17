using ETicaretAPI.Application.Abstractions.Services;
using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin;

public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandReqeust, FacebookLoginCommandResponse>
{

    readonly IAuthService _authService;

    public FacebookLoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandReqeust request, CancellationToken cancellationToken)
    {
        var token = await _authService.FacebookLoginAsync(request.AuthToken, 900);
        return new()
        {
            Token = token
        };
    }
}
