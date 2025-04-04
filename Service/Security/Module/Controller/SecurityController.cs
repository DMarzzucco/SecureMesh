using Microsoft.AspNetCore.Mvc;
using Security.Module.DTOs;
using Security.Module.Filter;
using Security.Module.Services.Interfaces;
using Security.Server.Model;

namespace Security.Module.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _service;

        public SecurityController(ISecurityService service)
        {
            _service = service;
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [ServiceFilter(typeof(LocalAuthFilter))]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO body)
        {
            var user = HttpContext.Items["User"] as UserModel;
            var newToken = await this._service.GenerateToken(user);

            return StatusCode(StatusCodes.Status200OK, new { token = newToken });
        }
        /// <summary>
        /// Get User Profile 
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<ActionResult<UserModel>> GetProfile()
        {
            var user = await this._service.GetProfile();
            return Ok(user);
        }

        /// <summary>
        /// Close Section
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<ActionResult> CloseSection()
        {
            await this._service.LogOut();
            return Ok(new { message = "Secction was closed successfullly" });
        }
        /// <summary>
        /// Refreh Token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken()
        {
            await this._service.GenerateRefreshToken();
            return NoContent();
        }
    }
}
