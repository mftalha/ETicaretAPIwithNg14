﻿using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Admin")]
public class ApplicationServicesController : ControllerBase
{
    readonly IApplicationService _applicationService;

    public ApplicationServicesController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition ="Get Authorize Definition EndPoints", Menu ="Application Services")]
    public IActionResult GetAuthorizeDefinitionEndPoints()
    {
        var datas =_applicationService.GetAuthorizeDefinitionEndpoints(typeof(Program));
        return Ok(datas);
    }
}
