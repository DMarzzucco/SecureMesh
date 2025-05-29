using System.Text;
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
        /// Regisred User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("registered")]
        public async Task<ActionResult<string>> Registered([FromBody] CreateUserDTO body)
        {
            var res = await this._service.RegisteredUser(body);
            return Ok(res);
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
        [HttpPost("123bsdnN2310000qwe123")]
        public async Task<ActionResult> RefreshToken()
        {
            await this._service.GenerateRefreshToken();
            return NoContent();
        }
        /// <summary>
        /// ForgetPassword
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("elm23019_123mskw_123fnsk")]
        public async Task<ActionResult<UserModel>> ForgetPassword([FromBody] ForgetPasswordDTO dto)
        {
            return Ok(await this._service.ForgetPassword(dto));
        }
        /// <summary>
        /// verification email
        /// </summary>
        /// <param name="kl124">Token</param>
        /// <returns></returns>
        [HttpGet("12349smska_wqj1n234msm949401")]
        public async Task<ActionResult<string>> VerifyEmail(string kl124)
        {
            return Ok(await this._service.VerificationEmail(kl124));
        }

        /// <summary>
        /// verification new email
        /// </summary>
        /// <param name="klt1276">Token</param>
        /// <returns></returns>
        [HttpGet("5413444_dsdn123fS_231_ddf")]
        public async Task<ActionResult<string>> VerifyNewEmail(string klt1276)
        {
            return Ok(await this._service.VerificationNewEmail(klt1276));
        }
        /// <summary>
        /// Remove Own Account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("remove_ownaccount/{id}")]
        public async Task<ActionResult<string>> RemoveOwnAccountAsync(int id, [FromBody] PasswordDTO dto)
        {
            return Ok(await this._service.RemoveOwnAccount(id, dto));
        }
        /// <summary>
        /// Change Email Adress
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPatch("{id}/l23_34201")]
        public async Task<ActionResult<string>> ReformEmailAddres(int id, [FromBody] NewEmailDTO body)
        {
            return Ok(await this._service.ChangeAddressEmail(id, body));
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="hmk12">Token</param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPatch("8382fd_1231sfw13312saeDAs12")]
        public async Task<ActionResult<string>> ReturningPassword(string hmk12, [FromBody] PasswordDTO body)
        {
            return Ok(await this._service.ResetPassword(hmk12, body));
        }
    }
}
