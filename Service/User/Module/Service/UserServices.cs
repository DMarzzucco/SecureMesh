using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.OpenApi.Expressions;
using User.Module.DTOs;
using User.Module.Model;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
using User.Module.Validations.Interface;
using User.Utils.Exceptions;

namespace User.Module.Service
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserValidation _validation;
        public UserServices(IUserRepository repository, IMapper mapper, IUserValidation validation)
        {
            _repository = repository;
            _mapper = mapper;
            _validation = validation;
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
            await this._validation.ValidationDuplicated(body);
            this._validation.ValidationEmail(body.Email);
            this._validation.ValidateStructurePassword(body.Password);

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
            var user = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

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
            var user = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

            user.RefreshToken = RefreshToken;
            await this._repository.UpdateAsync(user);
            return user;
        }

        /// <summary>
        /// Update Password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> UpdatePassword(int id, string oldPassword, string password)
        {
            var date = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

            var passwordHasher = new PasswordHasher<UserModel>();

            var verificationPass = passwordHasher.VerifyHashedPassword(date, date.Password, oldPassword);
            if (verificationPass == PasswordVerificationResult.Failed)
                throw new BadRequestExceptions("Password Wrong");

            this._validation.ValidateStructurePassword(password);

            var verificationResult = passwordHasher.VerifyHashedPassword(date, date.Password, password);

            if (verificationResult == PasswordVerificationResult.Success)
                throw new ConflictExceptions("The password cannot be the same as the current one");

            date.Password = passwordHasher.HashPassword(date, password);
            await this._repository.UpdateAsync(date);

            return "Password updated successfully";
        }
        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="body"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> UpdateRegister(UpdateUserDTO body, int id)
        {
            var user = await this._repository.FindByIdAsync(id) ?? 
                throw new NotFoundExceptions("User not found");

            this._validation.ValidationEmail(body.Email);
            
            this._mapper.Map(body, user);
            await this._repository.UpdateAsync(user);
            return user;
        }
    }
}
