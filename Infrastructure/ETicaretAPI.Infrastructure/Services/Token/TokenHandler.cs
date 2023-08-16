using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ETicaretAPI.Infrastructure.Services.Token;
// {Handler : Eğitici , işleyici}
public class TokenHandler : ITokenHandler
{
	readonly IConfiguration _configuration;

	public TokenHandler(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public Application.DTOs.Token CreateAccessToken(int second, AppUser user)
	{
		Application.DTOs.Token token = new();

		//Security Key'in smetriğini alıyoruz
		SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

		// Şifrelenmiş kimliği oluşturuyoruz.
		SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

		//Oluşturulacak token ayarlarını veriyoruz. //{expiration : süre sonu} 
		token.Expiration = DateTime.UtcNow.AddSeconds(second);
		JwtSecurityToken securityToken = new(
			audience: _configuration["Token:Audience"],
			issuer: _configuration["Token:Issuer"],
			expires: token.Expiration,
			notBefore: DateTime.UtcNow, //bu token üretildikten kaç dakika sonra devreye girsin. : üretilir üretilmez şuan.
			signingCredentials: signingCredentials,
			claims: new List<Claim> { new(ClaimTypes.Name, user.UserName) } //loglama için eklendi
			);

		//Token oluturucu sınıfından bir örnek alalım.
		JwtSecurityTokenHandler tokenHandler = new();
		token.AccessToken = tokenHandler.WriteToken(securityToken);

		token.RefreshToken = CreateRefreshToken();

		return token;
	}

    public string CreateRefreshToken()
    {
		byte[] number = new byte[32];

		// scoplar içinde using'i kullanmadığımzıda(eskiden direk scoplar içinde kullanılacak.) üsttei scop kapanınca yok edilecek ilgili nesne bellekten. : CreateRefreshToken tagı kapandığında silinecek şimdi.
        using RandomNumberGenerator random = RandomNumberGenerator.Create();
		random.GetBytes(number);
		return Convert.ToBase64String(number); // random değer üretildi ve stringe dönüştürülüp döndürüldü.

    }
}
