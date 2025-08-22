using System;
using IdentifyService._2FA.Interfaces;
using IdentifyService.Module.Model;
using IdentifyService.Module.Repository.Interface;
using IdentifyService.Utils.Exceptions;

namespace IdentifyService._2FA;

public class ValidateTwoFactorAuth(IIdentityProviderRepository repository) : IValidateTwoFactorAuth
{
    private readonly IIdentityProviderRepository repository = repository;

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
            throw new TooManyRequestsException("Account locked due to multiple failed attemps. Try again later");

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
