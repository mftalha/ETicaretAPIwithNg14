using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AppUser.AssignRoleToUser;

public class GetAppUsersQueryRequest : IRequest<GetAppUsersQueryResponse>
{
    public int Page { get; set; } = 0;
    public int Size { get; set; } = 0;
}