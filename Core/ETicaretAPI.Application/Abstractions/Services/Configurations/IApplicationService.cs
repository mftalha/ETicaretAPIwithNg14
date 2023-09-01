using ETicaretAPI.Application.DTOs.Configuration;

namespace ETicaretAPI.Application.Abstractions.Services.Configurations;

// uygulamayla ilgili serviş işlemleri için => veri ile ilgili değil
public interface IApplicationService
{
    List<Menu> GetAuthorizeDefinitionEndpoints(Type type);
}
