using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
{
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;
    readonly private SignInManager<Domain.Entities.Identity.AppUser> _signInManager; //role processing for service
    readonly private ITokenHandler _tokenHandler;

	public LoginUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, SignInManager<Domain.Entities.Identity.AppUser> signInManager,
        ITokenHandler tokenHandler)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_tokenHandler = tokenHandler;
	}

	public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
    {
        
        Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
        if (user == null)
            user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        if (user == null)
            throw new NotFoundUserException();

        // lockoutOnFailure ; CheckPasswordSignInAsync :3. paremetre : true ise şifre yanlış oldugunda kullanıcıyı kilitler.
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password,false);

        if (result == SignInResult.Success) //Authentication başarılı
        {
            // Yetkileri belirlememiz gerekiyor
            Token token = _tokenHandler.CreateAccessToken(5);
			return new LoginUserSuccessCommandResponse()
			{
				Token = token
		    };
	    }
		//return new LoginUserErrorCommandResponse()
		//{
		//	Message = "Kullanıcı adı veya şifre hatalı..."
		//};
		throw new AuthenticationErrorException(); //üsttekide olur bu da olur.
	}
}
