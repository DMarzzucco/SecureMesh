using AutoMapper;
using Microsoft.AspNetCore.Identity;
using User.Module.DTOs;
using User.Module.Model;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
using User.Utils.Exceptions;

namespace User.Module.Service
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserServices(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        /// <summary>
        /// Find User By id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> FindUserById(int id)
        {
            var user = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");
            return user;
        }
        /// <summary>
        /// Get User Profile By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserDTO> GetUserProfileById(int id)
        {
            var user = await this.FindUserById(id);
            var response = this._mapper.Map<UserDTO>(user);
            return response;
        }
        /// <summary>
        /// List of All Register
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<UserDTO>> ListOfAllRegister()
        {
            var user = await this._repository.ToListAsync();
            var response = this._mapper.Map<IEnumerable<UserDTO>>(user);
            return response;
        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="ConflictExceptions"></exception>
        public async Task<UserModel> RegisterUser(CreateUserDTO body)
        {
            if (this._repository.ExistisByUsername(body.Username))
                throw new ConflictExceptions("Username already exist");

            if (this._repository.ExistisByEmail(body.Email))
                throw new ConflictExceptions("Email already exist");

            var user = this._mapper.Map<UserModel>(body);

            var passwordHasher = new PasswordHasher<UserModel>();
            user.Password = passwordHasher.HashPassword(user, body.Password);

            await this._repository.AddChangeAsync(user);
            return user;
        }
        /// <summary>
        /// Remove User 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task RemoveUserRegister(int id)
        {
            var user = await this.FindUserById(id);
            await this._repository.DeleteAsync(user);
        }
        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> UpdateRefreshToken(int id, string RefreshToken)
        {
            var user = await this.FindUserById(id);
            user.RefreshToken = RefreshToken;
            await this._repository.UpdateAsync(user);
            return user;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> UpdateRegister(UpdateUserDTO body, int id)
        {
            var user = await this.FindUserById(id);
            this._mapper.Map(body, user);
            await this._repository.UpdateAsync(user);
            return user;
        }
    }
}
