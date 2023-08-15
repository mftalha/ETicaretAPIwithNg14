using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
}
