using User.Module.Model;

namespace User.Module.Repository.Interface
{
    public interface IUserRepository
    {
        Task<UserModel?> FindByIdAsync(int id);
        Task<UserModel?> FindByEmailAsync(string email);
        Task<IEnumerable<UserModel>> ToListAsync();
        Task<bool> ExistisByEmail(string email);
        Task <bool> ExistisByUsername(string username);
        Task AddChangeAsync(UserModel body);
        Task<bool> UpdateAsync(UserModel body);
        Task<bool> DeleteAsync(UserModel body);
        Task<UserModel?> FindByKey(string key, object value);
    }
}
