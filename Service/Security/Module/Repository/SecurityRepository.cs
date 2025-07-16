using System;
using Microsoft.EntityFrameworkCore;
using Security.Context;
using Security.Module.Model;
using Security.Module.Repository.Interfaces;

namespace Security.Module.Repository;

public class SecurityRepository(AppDbContext context) : ISecurityRepository
{
    private readonly AppDbContext context = context;

    /// <summary>
    /// Find Security relations by user id key
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SecurityModel> FindByUserId(int id)
    {
        var data = await this.context.SecurityModel.FirstOrDefaultAsync(s => s.UserId == id);

        if (data == null) return null;

        return data;
    }

    /// <summary>
    /// Find realtions secuity by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SecurityModel> FindRelationById(int id)
    {
        var data = await this.context.SecurityModel.FindAsync
        (id);

        if (data == null) return null;

        return data;
    }

    /// <summary>
    /// List of All sessions
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SecurityModel>> ListOfAllAsync()
    {
        return await this.context.SecurityModel.ToListAsync();
    }

    /// <summary>
    /// save realtions
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task SaveSession(SecurityModel body)
    {
        this.context.SecurityModel.Add(body);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// Update Session
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<bool> UpdateState(SecurityModel body)
    {
        this.context.SecurityModel.Entry(body).State = EntityState.Modified;
        await this.context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Remove realtions
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<bool> RemoveSessionAsync(SecurityModel body)
    {
        this.context.SecurityModel.Remove(body);
        await this.context.SaveChangesAsync();
        return true;
    }
}
