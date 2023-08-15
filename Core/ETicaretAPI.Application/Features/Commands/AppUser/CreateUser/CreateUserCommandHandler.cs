﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using MediatR;


namespace ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{
    readonly private IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        CreateUserResponseDTO response = await _userService.CreateAsync(new()
        {
            Email= request.Email,
            NameSurname= request.NameSurname,
            Password= request.Password,
            PasswordConfirm = request.PasswordConfirm,
            UserName= request.UserName
        });

        return new()
        {
            Message= response.Message,
            Succeeded = response.Succeeded
        };  
    }
}
