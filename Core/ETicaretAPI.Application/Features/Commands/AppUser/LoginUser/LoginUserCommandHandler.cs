using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
{
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;
    readonly private SignInManager<Domain.Entities.Identity.AppUser> _signInManager; //role processing for service

    public LoginUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, SignInManager<Domain.Entities.Identity.AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
    {
        
        Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
        if (user == null)
            user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        if (user == null)
            throw new NotFoundUserException("Kullanıcı veya şifre hatalı...");

        // lockoutOnFailure ; CheckPasswordSignInAsync :3. paremetre : true ise şifre yanlış oldugunda kullanıcıyı kilitler.
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password,false);

        if (result == SignInResult.Success) //Authentication başarılı
        {
            // Yetkileri belirlememiz gerekiyor
        }

        return null;
    }
}
