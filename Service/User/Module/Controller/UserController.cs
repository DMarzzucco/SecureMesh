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
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> EditOwnCount(int id, [FromBody] UpdateOwnUserDTO body)
        {
            var res = await this._service.UpdateOwnRegister(id, body);
            return Ok(res);
        }

        /// <summary>
        /// Update Password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        [HttpPatch("{id}/password")]
        public async Task<ActionResult<string>> UpdatePassword(int id, [FromBody] UpdatePasswordDTO dt)
        {
            return Ok(await this._service.UpdatePassword(id, dt));
        }

        /// <summary>
        /// Update Roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newRoles"></param>
        /// <returns></returns>
        [HttpPatch("{id}/rm0x1")]
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
    }
}
