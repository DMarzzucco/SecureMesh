using Microsoft.EntityFrameworkCore;
using User.Context;
using User.Module.Model;
using User.Module.Repository.Interface;

namespace User.Module.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Delete Async 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeleteAsync(UserModel body)
        {
            this._context.UserModel.Remove(body);
            await this._context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Exist by Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ExistisByEmail(string email)
        {
            return this._context.UserModel.Any(u => u.Email == email);
        }
        /// <summary>
        /// Exist by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ExistisByUsername(string username)
        {
            return this._context.UserModel.Any(u => u.Username == username);
        }
        /// <summary>
        /// Fin User by Value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel?> FindByIdAsync(int id)
        {
            return await this._context.UserModel.FindAsync(id);
        }
        /// <summary>
        /// Find User By Key and Value to validated
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel?> FindByKey(string key, object value)
        {
            var user = await this._context.UserModel
                   .AsQueryable()
                   .Where(u => EF.Property<object>(u, key).Equals(value))
                   .SingleOrDefaultAsync();
            return user;
        }
        /// <summary>
        /// Save User Register
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task AddChangeAsync(UserModel body)
        {
            this._context.UserModel.Add(body);
            await this._context.SaveChangesAsync();
        }
        /// <summary>
        /// List of all Register
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<UserModel>> ToListAsync()
        {
            return await this._context.UserModel.ToListAsync();
        }
        /// <summary>
        /// Update Register
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdateAsync(UserModel body)
        {
            this._context.UserModel.Entry(body).State = EntityState.Modified;
            await this._context.SaveChangesAsync();
            return true;
        }
    }
}
