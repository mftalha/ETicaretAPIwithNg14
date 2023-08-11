using ETicaretAPI.Application.Abstractions.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

	public Application.DTOs.Token CreateAccessToken(int minute)
	{
		Application.DTOs.Token token = new();

		//Security Key'in smetriğini alıyoruz
		SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

		// Şifrelenmiş kimliği oluşturuyoruz.
		SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

		//Oluşturulacak token ayarlarını veriyoruz. //{expiration : süre sonu} 
		token.Expiration = DateTime.UtcNow.AddMinutes(minute);
		JwtSecurityToken securityToken = new(
			audience: _configuration["Token:Audience"],
			issuer: _configuration["Token:Issuer"],
			expires: token.Expiration,
			notBefore: DateTime.UtcNow, //bu token üretildikten kaç dakika sonra devreye girsin. : üretilir üretilmez şuan.
			signingCredentials: signingCredentials
			);

		//Token oluturucu sınıfından bir örnek alalım.
		JwtSecurityTokenHandler tokenHandler = new();
		token.AccessToken = tokenHandler.WriteToken(securityToken);
		return token;
	}
}
