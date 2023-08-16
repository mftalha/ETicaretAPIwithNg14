using ETicaretAPI.Domain.Entities.Identity;

namespace ETicaretAPI.Application.Abstractions.Token;

public  interface ITokenHandler
{
    // , AppUser appUser => logg lamada kullanılmak üzere eklendi.
    DTOs.Token CreateAccessToken(int second, AppUser appUser); // token = jvt = access aynı şeyler
	string CreateRefreshToken();
}
