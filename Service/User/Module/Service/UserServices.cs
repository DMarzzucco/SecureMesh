using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

            if (user.Roles == ROLES.Admin)
                throw new ForbiddenExceptions("Could not delete a user Admin");

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
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestExceptions"></exception>
        /// <exception cref="NotFoundExceptions"></exception>
        /// <exception cref="ForbiddenExceptions"></exception>
        /// <exception cref="ConflictExceptions"></exception>
        public async Task<string> UpdatePassword(int id, UpdatePasswordDTO dt)
        {
            if (string.IsNullOrEmpty(dt.OldPassword))
                throw new BadRequestExceptions("Old password is required");

            var date = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

            var passwordHasher = new PasswordHasher<UserModel>();

            var verificationPass = passwordHasher.VerifyHashedPassword(date, date.Password, dt.OldPassword);
            if (verificationPass == PasswordVerificationResult.Failed)
                throw new ForbiddenExceptions("Password Wrong");

            this._validation.ValidateStructurePassword(dt.NewPassword);

            var verificationResult = passwordHasher.VerifyHashedPassword(date, date.Password, dt.NewPassword);

            if (verificationResult == PasswordVerificationResult.Success)
                throw new ConflictExceptions("The password cannot be the same as the current one");

            date.Password = passwordHasher.HashPassword(date, dt.NewPassword);
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

            if (user.Roles == ROLES.Admin)
                throw new ForbiddenExceptions("Could not edit a user Admin");

            this._validation.ValidationEmail(body.Email);

            this._mapper.Map(body, user);
            await this._repository.UpdateAsync(user);
            return user;
        }
        /// <summary>
        /// Update own register
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundExceptions"></exception>
        /// <exception cref="ForbiddenExceptions"></exception>
        public async Task<string> UpdateOwnRegister(int id, UpdateOwnUserDTO body)
        {
            if (string.IsNullOrEmpty(body.Password))
                throw new BadRequestExceptions("Password is required");

            var user = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

            var passwordHasher = new PasswordHasher<UserModel>();
            var verificationPass = passwordHasher.VerifyHashedPassword(user, user.Password, body.Password);
            if (verificationPass == PasswordVerificationResult.Failed)
                throw new ForbiddenExceptions("Password is wrong");

            this._validation.ValidationEmail(body.Email);

            this._mapper.Map(body, user);
            await this._repository.UpdateAsync(user);

            return "Your reforms was saved successfully";
        }
        /// <summary>
        /// Update Roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newRoles"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundExceptions"></exception>
        public async Task<string> UpdateRoles(int id, RolesDTO newRoles)
        {
            var user = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

            if (user.Roles == newRoles.Roles)
                throw new BadRequestExceptions("No changes were made, Roles is already set");

            user.Roles = newRoles.Roles;

            await this._repository.UpdateAsync(user);

            return "Roles were updated successfully";
        }

        /// <summary>
        /// Remove user for basic user roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> RemoveUserRegisterForBasicRoles(int id, PasswordDTO dt)
        {
            if (dt.Password == null)
                throw new BadRequestExceptions("Password is required");

            var user = await this._repository.FindByIdAsync(id) ??
                throw new NotFoundExceptions("User not found");

            var passwordHasher = new PasswordHasher<UserModel>();

            var verificationPass = passwordHasher.VerifyHashedPassword(user, user.Password, dt.Password);
            if (verificationPass == PasswordVerificationResult.Failed)
                throw new ForbiddenExceptions("Password Wrong");

            await this._repository.DeleteAsync(user);

            return "User was remove successfully";
        }
    }
}
