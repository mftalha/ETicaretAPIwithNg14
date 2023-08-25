using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ETicaretAPI.Persistence.Services;

public class UserService : IUserService
{
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreateUserResponseDTO> CreateAsync(CreateUserDTO model)
    {
        // UserManager => Kullanıcı işlemlerinden sorumlu servis : bu servisten dolayı biz user için repository oluşturmuyoruz çünkü arka planda zaten mevcut.

        IdentityResult result = await _userManager.CreateAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            Email = model.Email,
            NameSurname = model.NameSurname,
        }, model.Password);

        CreateUserResponseDTO response = new() { Succeeded = result.Succeeded };

        if (result.Succeeded)
            response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
        else
            foreach (var error in result.Errors)
                response.Message += $"{error.Code} - {error.Description}\n";

        //result.Errors.First(). ... => Burda hata varsa bize hatayıda döndürebiliyor.
        //throw new UserCreateFailedException();

        return response;
    }

    public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
    {
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);
            await _userManager.UpdateAsync(user);
        }
        else
            throw new NotFoundUserException();
    }

    public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
    {
        AppUser user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            //byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
            //resetToken = Encoding.UTF8.GetString(tokenBytes);

            resetToken = resetToken.UrlDecode();
            // şifre değişikliği için kullandığımız Identity methodu
            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
                //eğerki şifre değiştirme işlemi başarılı ise kullanıcının SecurityStamp değerini ezmemiz gerekiyor(değiştirme) => resettoken değeri birdahaki istekde farklı gelsin ve aynı reset token ile birdaha şifre değişikliği yapamasın diye.
                await _userManager.UpdateSecurityStampAsync(user);
            else
                throw new PasswordChangeFailedException();
        }
    }
}
