using Security.Server.Model;

namespace Security.Server.Service.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUserById(int id);
        Task UpdateRefreshToken(int id, string? refreshToken);
        Task<UserModel> FindByValue(string key, object value);
    }
}
