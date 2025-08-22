
using UserManagementService.Modules.Models;

namespace UserManagementService.Modules.Repository.Interfaces
{
    public interface IManagementUserRepository
    {
        Task SaveRelationManagementByUserId(int userId);
        Task<ManagementUserModel?> GetRelationManagementByUserId(int userId);
        Task DeleteRelationManagementByUserId(ManagementUserModel body);
        Task UpdateManagementUser(ManagementUserModel body);
    }
}
