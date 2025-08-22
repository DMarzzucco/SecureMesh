using System;
using UserManagementService.Server.Sessions.Models;
using UserManagementService.Server.Users.Model;

namespace UserManagementService.Modules.Services.Interfaces;

public interface IManagementUserServices
{
    Task<string> UpdateAnyCrendetial(int id, UpdateOwnRegisterDTO body);
    Task<string> UpdateEmailAdress(string token);
    Task<string> ForgetPasswordAccount(ForgetPasswordDTO dto);
    Task<string> ResetPassword(string token, PasswordDTO body);
}
