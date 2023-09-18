using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ETicaretAPI.API.Filters;

public class RolePermissionFilter : IAsyncActionFilter
{
	// user'a yakalıyacaz(Identity'den kaynaklı user zaten benzersiz => username'den) => code ile de ilgili endpoint'i yakalıyacaz =>=> bu user ile endpoint aynı rollere sahip mi değilmi ? bunu kontrol edecez

	readonly IUserService _userService;

	public RolePermissionFilter(IUserService userService)
	{
		_userService = userService;
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		// token dan alınan user name => token diye hatırlııyorum.
		var name = context.HttpContext.User.Identity?.Name;
		// burda koda gömdük admin hesabını ama user tablosana ek bir alan ekleyip adminmi diye => eğerki admin ise yine burdaki if'e girmeden direk tüm yetkilere sahip olacak.
		if (!string.IsNullOrEmpty(name) && name != "tlh")
		{
			// action la ilgili yüzeysel bilgileri => ActionDescriptor bu şekilde elde edebiliyorum
			// as ControllerActionDescriptor; => action name'de elde edebilmek için
			var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
			//ilgili actionun extensionlarını yakalıyabilkirim. => GetCustomAttributes
			var attribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

			// istek => get, put , post , delete => hangi http türü.
			var httpAttribute =  descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

			//eğerki http ile işaretlenmemiş ise get dir o.
			//veritabanına endpoint tablosundaki kod alanına attığımız formatı elde ettik.
			var code = $"{(httpAttribute != null ? httpAttribute.HttpMethods.First() : HttpMethods.Get)}.{attribute.ActionType}.{attribute.Definition.Replace(" ", "")}";

			var hasRole = await _userService.HasRolePermissionToEndpointAsync(name, code);

			//eğerki kod yoksa 404 döndürecez
			if (!hasRole)
				context.Result = new UnauthorizedResult();
			else //sonrakine geç
				await next();
		}
		else
			await next();
	}
}
