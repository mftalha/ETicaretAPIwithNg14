using ETicaretAPI.Application.DTOs.User;

namespace ETicaretAPI.Application.Abstractions.Services;

public interface IUserService
{
    Task<CreateUserResponseDTO> CreateAsync(CreateUserDTO model);
}
