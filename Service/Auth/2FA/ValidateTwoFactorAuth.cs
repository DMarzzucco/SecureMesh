using System;
using Auth._2FA.Interfaces;
using Auth.Module.Model;
using Auth.Module.Repository.Interface;
using Auth.Utils.Exceptions;

namespace Auth._2FA;

public class ValidateTwoFactorAuth(IAuthRepository repository) : IValidateTwoFactorAuth
{
    private readonly IAuthRepository repository = repository;

    /// <summary>
    /// Validate 2FA Code
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestExceptions"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="ForbiddenExceptions"></exception>
    public async Task<AuthModel> ImplementValidate(int userId, string code)
    {
        if (string.IsNullOrEmpty(code))
            throw new BadRequestExceptions("Code is required");

        var auth = await this.repository.FindAuthByUserId(userId) ??
            throw new UnauthorizedAccessException();

        if (auth.LockedAt != null && DateTime.UtcNow < auth.LockedAt)
            throw new ForbiddenExceptions("Account locked due to multiple failed attemps. Try again later");

        if (auth.TwoFACode != code || auth.TwoFACodeExpiration < DateTime.UtcNow)
        {
            auth.VerifyAttempts++;

            if (auth.VerifyAttempts >= 3)
            {
                auth.LockedAt = DateTime.UtcNow.AddMinutes(10);
                auth.VerifyAttempts = 0;
            }
            await this.repository.UpdateAsync(auth);
            throw new ForbiddenExceptions("Code is invalid or is expired");
        }

        auth.TwoFACode = Guid.NewGuid().ToString();
        auth.TwoFACodeExpiration = null;
        auth.VerifyAttempts = 0;
        auth.LockedAt = null;

        await this.repository.UpdateAsync(auth);

        return auth;
    }
}
