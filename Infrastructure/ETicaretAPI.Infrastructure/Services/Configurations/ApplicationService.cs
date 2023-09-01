using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configuration;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ETicaretAPI.Infrastructure.Services.Configurations;

// Role kontrolü yapılacak actionları bir liste şeklinde elde etmek için oluşturuldu. => araya girilip ilgili işlemelr yapılacak sonrasında.
// https://www.youtube.com/watch?v=QZYtrzGEcvo&list=PLQVXoXFVVtp1DFmoTL4cPTWEWiqndKexZ&index=64 dersinden.
public class ApplicationService : IApplicationService
{
    public List<Menu> GetAuthorizeDefinitionEndpoints(Type type)
    {
        // type => hangi katmana bakacaz => program veriyoruz başta api katmanında Program o yüzden ilgili katmana bakıyor gibi.
        Assembly assembly = Assembly.GetAssembly(type);
        //api katmanındaki controller'ları elde ediyoruz
        // ControllerBase => controller lar controllerlar ControllerBase'den türediği için : ayırt edicilik için.
        var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));

        List<Menu> menus = new();

        if (controllers != null)
            foreach(var controller in controllers)
            {
                // controller içindeki actionlar'dan AuthorizeDefinitionAttribute attributesi ile işaretlenmişleri getir.
                var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));
                if(actions != null)
                    foreach(var action in actions)
                    {
                        var attributes = action.GetCustomAttributes(true);
                        if(attributes != null)
                        {
                            Menu menu = null;

                            var authorizeDefinitionAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                            //menü yoksa oluşturuyoruz
                            if (!menus.Any(m => m.Name == authorizeDefinitionAttribute.Menu))
                            {
                                menu = new() { Name = authorizeDefinitionAttribute.Menu };
                                menus.Add(menu);
                            }
                            // menü varsa buradaki referansa menüyü veriyoruz
                            else
                                menu = menus.FirstOrDefault(m => m.Name == authorizeDefinitionAttribute.Menu);

                            // AuthorizeDefinitionAttribute içindfeki verileri elde ediyoruz
                            Application.DTOs.Configuration.Action _action = new()
                            {
                                // enum string değeri almak için
                                ActionType = Enum.GetName(typeof(ActionType), authorizeDefinitionAttribute.ActionType),
                                Definition = authorizeDefinitionAttribute.Definition,
                            };

                            // httpAttribute'ye Get,Post.Put gibi http türünü elde ediyoruz
                            var httpAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute;

                            // null değilse ilkini , null ise ilgili değeri alıyoruz. => default olarak : http atribute gelmedi ise.
                            if (httpAttribute != null)
                                _action.HttpType = httpAttribute.HttpMethods.First();
                            else
                                _action.HttpType = HttpMethods.Get;

                            _action.Code = $"{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ","")}";

                            menu.Actions.Add(_action);
                        }
                    }
            }
        return menus;
    }
}
