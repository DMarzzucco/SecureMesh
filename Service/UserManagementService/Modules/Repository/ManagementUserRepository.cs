using Microsoft.EntityFrameworkCore;
using UserManagementService.Context;
using UserManagementService.Modules.Models;
using UserManagementService.Modules.Repository.Interfaces;

namespace UserManagementService.Modules.Repository
{
    public class ManagementUserRepository(AppDbContext context) : IManagementUserRepository
    {
        private readonly AppDbContext context = context;

        /// <summary>
        /// Remove MS Relation
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DeleteRelationManagementByUserId(ManagementUserModel body)
        {
            this.context.ManagementUser.Remove(body);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Get MS Relation by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ManagementUserModel?> GetRelationManagementByUserId(int userId)
        {
            var ms = await this.context.ManagementUser.FirstOrDefaultAsync(ms => ms.UserId == userId);
            return ms;
        }

        /// <summary>
        /// Save Relation MS by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task SaveRelationManagementByUserId(int userId)
        {
            var body = new ManagementUserModel { UserId = userId };
            this.context.ManagementUser.Add(body);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Update MS
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task UpdateManagementUser(ManagementUserModel body)
        {
            this.context.ManagementUser.Entry(body).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }
    }
}
