using System;
using Auth.Module.Model;

namespace Auth._2FA.Interfaces;

public interface IValidateTwoFactorAuth
{
    Task<AuthModel> ImplementValidate(int userId, string code);
}
