using Microsoft.AspNetCore.Mvc;
using User.Module.DTOs;
using User.Module.Model;
using User.Module.Service.Interface;

namespace User.Module.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }
        /// <summary>
        /// Get All 
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllRegister()
        {
            return Ok(await this._service.ListOfAllRegister());
        }
        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            return Ok(await this._service.GetUserProfileById(id));
        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("xk12")]
        public async Task<ActionResult<UserModel>> RegisterUser([FromBody] CreateUserDTO body)
        {
            var date = await this._service.RegisterUser(body);
            return CreatedAtAction(nameof(GetAllRegister), new { id = date.Id }, date);
        }
        /// <summary>
        /// Edit User (ROLES:CREATOR)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPut("{id}/e90u")]
        public async Task<IActionResult> EditUser(int id, [FromBody] UpdateUserDTO body)
        {
            await this._service.UpdateRegister(body, id);
            return NoContent();
        }

        /// <summary>
        /// Update own count (ROLES:BASIC)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> EditOwnCount (int id, string password, [FromBody] UpdateUserDTO body)
        {
            var res =await this._service.UpdateOwnRegister(id, password, body);
            return Ok(res);
        }
        /// <summary>
        /// Update Password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPut("{id}/password")]
        public async Task<ActionResult<string>> UpdatePassword(int id, string oldPassword, string newPassword)
        {
            return Ok(await this._service.UpdatePassword(id, oldPassword, newPassword));
        }

        /// <summary>
        /// Update Roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newRoles"></param>
        /// <returns></returns>
        [HttpPut("{id}/rm0x1")]
        public async Task<ActionResult<string>> UpdateRoles(int id, [FromBody] RolesDTO newRoles)
        {
            var res = await this._service.UpdateRoles(id, newRoles);
            return Ok(res);
        }

        /// <summary>
        /// Remove User Creator
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}/r37d")]
        public async Task<IActionResult> DeleteRegister(int id)
        {
            await this._service.RemoveUserRegister(id);
            return NoContent();
        }

        /// <summary>
        /// Delete Own Count 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpDelete("{id}/deletedCount")]
        public async Task <ActionResult<string>> DeleteOwnCount (int id, string password)
        {
            return Ok(await this._service.RemoveUserRegisterForBasicRoles(id, password));
        }
    }
}
