using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint;
using ETicaretAPI.Application.Features.Queries.AuthorizationEndpoint.GetRolesToEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers;

//seçilen rollerile end-pointler ile ilişkilendirme

[Route("api/[controller]")]
[ApiController]
public class AuthorizationEndpointsController : ControllerBase
{
	readonly IMediator _mediator;

	public AuthorizationEndpointsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> GetRolesToEndpoint(GetRolesToEndpointsQueryRequest getRolesToEndpointsQueryRequest)
	{
		GetRolesToEndpointsQueryResponse response = await _mediator.Send(getRolesToEndpointsQueryRequest);
		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> AssignRoleEndpoint(AssignRoleEndpointCommandRequest assignRoleEndpointCommandRequest)
	{
		assignRoleEndpointCommandRequest.Type = typeof(Program);
		AssignRoleEndpointCommandResponse response = await _mediator.Send(assignRoleEndpointCommandRequest);
		return Ok(response);
	}
}
