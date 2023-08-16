namespace ETicaretAPI.Application.Abstractions.Token;

public  interface ITokenHandler
{
	DTOs.Token CreateAccessToken(int second); // token = jvt = access aynı şeyler
	string CreateRefreshToken();
}
