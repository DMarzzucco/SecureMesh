using System;
using IdentifyService.Server.UMS.Model;
using IdentifyService.Server.UMS.Model;

namespace IdentifyService.Server.UMS.Services.Interfaces;

public interface IManagementUserFacedeServices
{
    Task<UserModel> SaveUserRegistered(CreateUserDTO body);
    Task<UserModel> FindUserById(int id);
    Task<UserModel> FindByValue(string username);
    Task<UserModel> FindUserByEmail(string email);
    Task<string> UpdatePasswordUser(int userId, int sessionId, UpdatePasswordDTO body);
    Task<UserModel> VerifyNewEmailParameters(int userId, NewEmailDTO body);
    Task<IEnumerable<SessionModel>> FindAllSessionsByUserId(int userId);
    Task<SessionModel> FindSessionIfExists(int userId, string ip, string userAgent, string location);
    Task<int> SaveSessionRegister(int userId, string ip, string userAgent, string location);
    Task RemoveSessionById(int sessionId);
    Task RequestToRemoveOwnAccount(int userId);
    Task CancelRemoveAccountOperationIfOn(int userId);

}
