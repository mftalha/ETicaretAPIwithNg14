using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandReqeust, GoogleLoginCommandResponse>
{
	readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;
	readonly private ITokenHandler _tokenHandler;

	public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler)
	{
		_userManager = userManager;
		_tokenHandler = tokenHandler;
	}

	public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandReqeust request, CancellationToken cancellationToken)
	{
		var settings = new GoogleJsonWebSignature.ValidationSettings()
		{
			// gogle hesapdaki lient id => client projesindede bu id yi girdim
			Audience = new List<string> { "408152542383-imii4i189nmj3db14e5rbl3p1old1037.apps.googleusercontent.com" }
		};

		//ilgili setting'i idtoken ile doğrula doğruysa payload'ları elde edebilecez
		var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

		// biz normalde kullanıcılar aspnetuser tablosuna kaydediyprız ama dış kaynaklardan eklenen kullanıcıları : aspnetuserlogins tablosuna kaydediyoruz. : eğerki yoksa
		var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);

		// user'ın kayıtlı olup olmadığını kontrol ediyoruz tablodan
		Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

		bool result = user != null;
		if(user == null)
		{
			// bu mailde bir kullanıcı varmı diye ekstra sorgulama yapıyoruz.
			user = await _userManager.FindByEmailAsync(payload.Email);
			if(user == null )
			{
				user = new()
				{
					Id = Guid.NewGuid().ToString(),
					Email = payload.Email,
					UserName = payload.Email,
					NameSurname = payload.Name
				};
				//asp.net user tablosuna kaydetme
				var identityResult = await _userManager.CreateAsync(user);
				result = identityResult.Succeeded;
			}
		}
		//asp.netuserLogins tablosuna ekliyoruz => dış kayıt olduğu için (google,face,ins)
		if (result)
			await _userManager.AddLoginAsync(user, info);
		else
			throw new Exception("Invalid external authentication.");

		Token token = _tokenHandler.CreateAccessToken(5);
			
		return new()
		{
			Token = token
		};
	}
}



