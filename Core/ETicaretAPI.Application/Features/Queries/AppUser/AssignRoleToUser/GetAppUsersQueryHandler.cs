using ETicaretAPI.Application.Abstractions.Services;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AppUser.AssignRoleToUser;

public class GetAppUsersQueryHandler : IRequestHandler<GetAppUsersQueryRequest, GetAppUsersQueryResponse>
{
    readonly IUserService _userService;

    public GetAppUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<GetAppUsersQueryResponse> Handle(GetAppUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(request.Page, request.Size);

        return new()
        {
            Users = users,
            TotalUsersCount = _userService.TotalUsersCount
        };
    }
}
