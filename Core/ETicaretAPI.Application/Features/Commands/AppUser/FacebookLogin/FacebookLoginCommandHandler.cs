using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin;

public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandReqeust, FacebookLoginCommandResponse>
{
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;
    readonly ITokenHandler _tokenHandler;
    readonly HttpClient _httpClient;

    public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandReqeust request, CancellationToken cancellationToken)
    {
        // facebook ile api'yi bağladık
        string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id=1038169094012077&client_secret=ffe591e8ab42b1401d2348da43f444f1&grant_type=client_credentials");

        FacebookAccessTokenResponseDTO facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponseDTO>(accessTokenResponse);

        // kullanıcının uygulamaya login olup olmadığını kontrol ediyoruz.
        // input_token => kullanııdan gelen token;; access_token => uygulamadan gelen token
        string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookAccessTokenResponse.AccessToken}");

        FacebookUserAccessTokenValidationDTO validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidationDTO>(userAccessTokenValidation);

        //kullanıcı doğru ise
        if (validation.Data.IsValid)
        {
            string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");

            FacebookUserInfoResponseDTO userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponseDTO>(userInfoResponse);


            var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");

            // user'ın kayıtlı olup olmadığını kontrol ediyoruz tablodan
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null;
            if (user == null)
            {
                // bu mailde bir kullanıcı varmı diye ekstra sorgulama yapıyoruz.
                user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = userInfo.Email,
                        UserName = userInfo.Email,
                        NameSurname = userInfo.Name
                    };
                    //asp.net user tablosuna kaydetme
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }
            //asp.netuserLogins tablosuna ekliyoruz => dış kayıt olduğu için (google,face,ins)
            if (result)
            {
                await _userManager.AddLoginAsync(user, info);

                Token token = _tokenHandler.CreateAccessToken(5);
                return new()
                {
                    Token = token
                };
            }
        }
        throw new Exception("Invalid external authentication.");
    }
}
