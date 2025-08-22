using System;
using IdentifyService.Context;
using IdentifyService.Module.Model;
using IdentifyService.Module.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace IdentifyService.Module.Repository;

public class IdentityProviderRepository(AppDbContext context) : IIdentityProviderRepository
{
    private readonly AppDbContext context = context;

    /// <summary>
    /// Find Auth for Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<AuthModel?> FindAuthByUserId(int userId)
    {
        return await this.context.AuthModel.FirstOrDefaultAsync(a => a.UserId == userId);
    }

    /// <summary>
    /// Remove Auth Realtion Async 
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task RemoveAuthRealtionAsync(AuthModel body)
    {
        this.context.Remove(body);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    ///  Save auth register
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task SaveAuth(int userId)
    {
        var body = new AuthModel { UserId = userId };

        this.context.AuthModel.Add(body);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// Update Auth
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(AuthModel body)
    {
        this.context.AuthModel.Entry(body).State = EntityState.Modified;
        await this.context.SaveChangesAsync();
        return true;
    }

}
