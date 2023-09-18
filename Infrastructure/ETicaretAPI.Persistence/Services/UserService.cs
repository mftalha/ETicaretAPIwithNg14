using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services;

public class UserService : IUserService
{
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;
    readonly private IEndpointReadRepository _endpointReadRepository;

	public UserService(UserManager<AppUser> userManager, IEndpointReadRepository endpointReadRepository)
	{
		_userManager = userManager;
		_endpointReadRepository = endpointReadRepository;
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

	public async Task<List<ListUser>> GetAllUsersAsync(int page, int size)
    {
        var users = await _userManager.Users
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return users.Select(user => new ListUser
        {
            Id = user.Id,
            Email = user.Email,
            NameSurname = user.NameSurname,
			TwoFactorEnabled = user.TwoFactorEnabled,
			UserName = user.UserName
        }).ToList();
    }

	public int TotalUsersCount => _userManager.Users.Count();

	public async Task AssignRoleToUserAsync(string userId, string[] roles)
	{
        AppUser user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // kullanıcının mevcut rolelrini siliyoruz
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            await _userManager.AddToRolesAsync(user, roles);
        }
        // kullanıcı yok ise hata fırlatılmalı
	}

	public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
	{
        AppUser user = await _userManager.FindByIdAsync(userIdOrName);
        if (user == null)
			user = await _userManager.FindByNameAsync(userIdOrName);

		if (user != null)
        {
            var userRoles =  await _userManager.GetRolesAsync(user);
            return userRoles.ToArray();
		}
        return new string[] { };
	}

	public async Task<bool> HasRolePermissionToEndpointAsync(string name, string code)
	{
        var userRoles = await GetRolesToUserAsync(name);

        //hiç rol yoksa false döndürüyoruz
        if(!userRoles.Any()) 
            return false;

        Endpoint? endpoint = await _endpointReadRepository.Table
            .Include(e => e.Roles)
            .FirstOrDefaultAsync(e => e.Code == code);

        if(endpoint == null)
            return false;

        var hasRole = false;
        var endpointRoles = endpoint.Roles.Select(r => r.Name);

        // çift for yerine daha düzgün algoritmalar kullanılabilir.

        foreach(var userRole in userRoles)
        {
            foreach (var endpointRole in endpointRoles)
                if (userRole == endpointRole)
                    return true;
        }
        return false;
	}
}
