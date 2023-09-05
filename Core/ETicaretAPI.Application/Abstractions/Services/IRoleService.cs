namespace ETicaretAPI.Application.Abstractions.Services;

public interface IRoleService
{
	// dto kullanılabilir object yerine
	(object, int) GetAllRoles(int page, int size);
    Task<(string id, string name)> GetRoleById(string id);
    Task<bool> CreateRole(string name);
    Task<bool> DeleteRole(string id);
    Task<bool> UpdateRole(string id, string name);
}
