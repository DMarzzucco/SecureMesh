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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetAllRegister()
        {
            return Ok(await this._service.ListOfAllRegister());
        }
        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserById (int id)
        {
            return Ok(await this._service.FindUserById(id));
        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserModel>> RegisterUser([FromBody] CreateUserDTO body)
        {
            var date = await this._service.RegisterUser(body);
            return CreatedAtAction(nameof(GetAllRegister), new { id = date.Id }, date);
        }
        /// <summary>
        /// Edit User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody]UpdateUserDTO body) {
            await this._service.UpdateRegister(body, id);
            return NoContent();
        }
        /// <summary>
        /// Remove User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegister(int id) {
            await this._service.RemoveUserRegister(id);
            return NoContent();
        }
    }
}
