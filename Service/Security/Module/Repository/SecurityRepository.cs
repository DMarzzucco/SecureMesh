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
    public async Task<SessionModel?> FindByUserId(int id)
    {
        return await this.context.SessionModel.FirstOrDefaultAsync(s => s.UserId == id); ;
    }

    /// <summary>
    /// Find Session
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<SessionModel?> FindSession(int userId, string ip, string userAgent, string location)
    {
        return await this.context.SessionModel
            .FirstOrDefaultAsync(s => s.UserId == userId
                && s.Ip == ip
                && s.UserAgent == userAgent
                && s.Location == location
                );
    }

    /// <summary>
    /// Find realtions secuity by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SessionModel?> FindSessionById(int id)
    {
        return await this.context.SessionModel.FindAsync
        (id);
    }

    /// <summary>
    /// List of All sessions
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SessionModel>> FindAllSessionsByUserId(int userId)
    {
        return await this.context.SessionModel
            .Where(s => s.UserId == userId).ToListAsync();
    }

    /// <summary>
    /// save realtions
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task SaveSession(SessionModel body)
    {
        this.context.SessionModel.Add(body);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// Update Session
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<bool> UpdateState(SessionModel body)
    {
        this.context.SessionModel.Entry(body).State = EntityState.Modified;
        await this.context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Remove realtions
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<bool> RemoveSessionAsync(SessionModel body)
    {
        this.context.SessionModel.Remove(body);
        await this.context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Remove all Session except Current
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="currentSessoinId"></param>
    /// <returns></returns>
    public async Task RemoveAllSessionsExceptCurrent(int userId, int currentSessoinId)
    {
        var sessionToDelete = await this.context.SessionModel
            .Where(s => s.UserId == userId && s.Id != currentSessoinId)
            .ToListAsync();

        this.context.SessionModel.RemoveRange(sessionToDelete);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove all Sessions by user Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task RemoveAllSessionsByUserId(int userId)
    {
        var sessions = await this.context.SessionModel
            .Where(s => s.UserId == userId)
            .ToListAsync();

        this.context.RemoveRange(sessions);
        await this.context.SaveChangesAsync();
    }
}
