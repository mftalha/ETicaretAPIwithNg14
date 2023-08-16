using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ETicaretAPI.Persistence.Services;

public class AuthService : IAuthService
{
    readonly HttpClient _httpClient;
    readonly IConfiguration _configuration;
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;
    readonly ITokenHandler _tokenHandler;
    readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
    readonly IUserService _userService;

    public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, SignInManager<AppUser> signInManager, IUserService userService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _signInManager = signInManager;
        _userService = userService;
    }
    // CreateUserExternalAsync => FacebookLoginAsync, GoogleLoginAsync için ortak işlemler için
    async Task<Token> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo info, int accessTokenLifeTime)
    {
        bool result = user != null;
        if (user == null)
        {
            // bu mailde bir kullanıcı varmı diye ekstra sorgulama yapıyoruz.
            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    UserName = email,
                    NameSurname = name
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

            Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
            await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);

            return token;
        }
        throw new Exception("Invalid external authentication.");
    }

    public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
    {
        // facebook ile api'yi bağladık
        string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:Client_ID"]}&client_secret={_configuration["ExternalLoginSettings:Facebook:Client_Secret"]}&grant_type=client_credentials");

        FacebookAccessTokenResponseDTO? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponseDTO>(accessTokenResponse);

        // kullanıcının uygulamaya login olup olmadığını kontrol ediyoruz.
        // input_token => kullanııdan gelen token;; access_token => uygulamadan gelen token
        string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

        FacebookUserAccessTokenValidationDTO? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidationDTO>(userAccessTokenValidation);

        //kullanıcı doğru ise
        if (validation?.Data.IsValid != null)
        {
            string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");

            FacebookUserInfoResponseDTO? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponseDTO>(userInfoResponse);

            var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");

            // user'ın kayıtlı olup olmadığını kontrol ediyoruz tablodan
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, info, accessTokenLifeTime);
        }
        throw new Exception("Invalid external authentication.");
    }

    public async Task<Token> GoogleLoginAsync(string IdToken, int accessTokenLifeTime)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            // gogle hesapdaki lient id => client projesindede bu id yi girdim
            Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_ID"] }
        };

        //ilgili setting'i idtoken ile doğrula doğruysa payload'ları elde edebilecez
        var payload = await GoogleJsonWebSignature.ValidateAsync(IdToken, settings);

        // biz normalde kullanıcılar aspnetuser tablosuna kaydediyprız ama dış kaynaklardan eklenen kullanıcıları : aspnetuserlogins tablosuna kaydediyoruz. : eğerki yoksa
        var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");

        // user'ın kayıtlı olup olmadığını kontrol ediyoruz tablodan
        Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);
    }

    public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
    {
        Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
        if (user == null)
            user = await _userManager.FindByEmailAsync(usernameOrEmail);

        if (user == null)
            throw new NotFoundUserException();

        // lockoutOnFailure ; CheckPasswordSignInAsync :3. paremetre : true ise şifre yanlış oldugunda kullanıcıyı kilitler.
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (result == SignInResult.Success) //Authentication başarılı
        {
            // Yetkileri belirlememiz gerekiyor
            Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
            await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);
            return  token;
        }
        //return new LoginUserErrorCommandResponse()
        //{
        //	Message = "Kullanıcı adı veya şifre hatalı..."
        //};
        throw new AuthenticationErrorException(); //üsttekide olur bu da olur.
    }

    public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
    {
        AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
        {
            Token token = _tokenHandler.CreateAccessToken(15);
            await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);
            return token;
        }
        else
            throw new NotFoundUserException();
    }
}
