using System;
using IdentifyService.Module.Model;

namespace IdentifyService._2FA.Interfaces;

public interface IValidateTwoFactorAuth
{
    Task<AuthModel> ImplementValidate(int userId, string code);
}
