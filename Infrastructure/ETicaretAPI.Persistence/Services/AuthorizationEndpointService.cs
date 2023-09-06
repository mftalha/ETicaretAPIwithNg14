using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services;

public class AuthorizationEndpointService : IAuthorizationEndpointService
{
	readonly IApplicationService _applicationService;
	readonly IEndpointReadRepository _endpointReadRepository;
	readonly IEndpointWriteRepository _endpointWriteRepository;
	readonly IMenuReadRepository _menuReadRepository;
	readonly IMenuWriteRepository _menuWriteRepository;
	readonly RoleManager<AppRole> _roleManager;

	public AuthorizationEndpointService(IApplicationService applicationService, IEndpointReadRepository endpointReadRepository, IMenuReadRepository menuReadRepository, IMenuWriteRepository menuWriteRepository, IEndpointWriteRepository endpointWriteRepository, RoleManager<AppRole> roleManager)
	{
		_applicationService = applicationService;
		_endpointReadRepository = endpointReadRepository;
		_endpointWriteRepository = endpointWriteRepository;
		_menuReadRepository = menuReadRepository;
		_menuWriteRepository = menuWriteRepository;
		_roleManager = roleManager;
	}

	public async Task AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type)
	{
		//menü yoksa menüyü ekliyoruz
		Menu _menu = await _menuReadRepository.GetSingleAsync(m => m.Name == menu);
		if (_menu == null)
		{
			_menu = new()
			{
				Id = Guid.NewGuid(),
				Name = menu
			};

			await _menuWriteRepository.AddAsync(_menu);
			await _menuWriteRepository.SaveAsync();
		}

		// Endpoint yoksa endpoint'i ekliyoruz.
		Endpoint endpoint =  await _endpointReadRepository.Table.Include(e => e.Menu).Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);

		if(endpoint == null)
		{
			var action = _applicationService.GetAuthorizeDefinitionEndpoints(type).FirstOrDefault(m => m.Name == menu)?
				.Actions.FirstOrDefault(e => e.Code == code);

			endpoint = new()
			{
				Code = action.Code,
				ActionType = action.ActionType,
				HttpType = action.HttpType,
				Definition = action.Definition,
				Id = Guid.NewGuid(),
				Menu= _menu,
			};

			await _endpointWriteRepository.AddAsync(endpoint);
			await _endpointWriteRepository.SaveAsync();
		}

		//Endpoint'in roller ile ilişkisini kopartıyorum => daha önceden endpoint ile ilişkili rol varsa ve biz onlar yerine yenilerini ekliyorsak => öncekinler kopacak yenileri zaten yukarıdan alıyoruz ve aşşağıda tekrardan ilişkilendireceğiz  => böylece checkbox üzerinden daha önce ilişkilendirilip sonradan seçimi kaldırılan rolelrinde ilişkisi rahatça kopartılacabilecek.
		foreach (var role in endpoint.Roles)
			endpoint.Roles.Remove(role);

		//Endpoint'i roller ile ilişkilendiriyorum
		// paremtre olarak gelen rol isimlerinin bağlı olduğu rolleri alıyoruz.
		var appRoles = await _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();

		foreach (var role in appRoles)
			endpoint.Roles.Add(role);

		await _endpointWriteRepository.SaveAsync();
	}

	public async Task<List<string>> GetRolesToEndpointAsync(string code, string menu)
	{
		Endpoint? endpoint = await _endpointReadRepository.Table
			.Include(e => e.Roles)
			.Include(e => e.Menu)
			.FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);

		if (endpoint != null)
			return endpoint.Roles.Select(r => r.Name).ToList();
		else
			return null;
	}
}
