using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{
    readonly private UserManager<Domain.Entities.Identity.AppUser> _userManager;

    public CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        // UserManager => Kullanıcı işlemlerinden sorumlu servis : bu servisten dolayı biz user için repository oluşturmuyoruz çünkü arka planda zaten mevcut.

        IdentityResult result = await _userManager.CreateAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName= request.UserName,
            Email= request.Email,
            NameSurname= request.NameSurname,
        },request.Password);

        CreateUserCommandResponse response = new() { Succeeded = result.Succeeded };

        if (result.Succeeded)
             response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
        else
        {
            foreach( var error in result.Errors)
                response.Message += $"{error.Code} - {error.Description}\n";
        }

        return response;

        //result.Errors.First(). ... => Burda hata varsa bize hatayıda döndürebiliyor.
        //throw new UserCreateFailedException();




    }
}
