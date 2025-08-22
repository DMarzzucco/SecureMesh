using System.Text;
using Microsoft.AspNetCore.Mvc;
using IdentifyService.Module.DTOs;
using IdentifyService.Module.Filter;
using IdentifyService.Module.Services.Interfaces;
using IdentifyService.Server.UMS.DTOs;
using IdentifyService.Server.UMS.Model;
using IdentifyService.Server.UMS.Model;

namespace IdentifyService.Module.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdpController(IIdentityProviderService service) : ControllerBase
    {
        private readonly IIdentityProviderService _service = service;

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
        [HttpPut("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO body)
        {
            var user = HttpContext.Items["User"] as UserModel ??
                throw new ArgumentNullException();

            var response = await this._service.Login(user);

            return StatusCode(StatusCodes.Status200OK, new { message = response });
        }

        /// <summary>
        /// Verify code 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("init-session")]
        public async Task<ActionResult<string>> StartSession([FromBody] VerifyCodeDTO body)
        {
            return Ok(await this._service.InitSession(body));
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
        /// Get all sessions by user
        /// </summary>
        /// <returns></returns>
        [HttpGet("sessions-list")]
        public async Task<ActionResult<IEnumerable<SessionModel>>> GetAllSessions()
        {
            return Ok(await this._service.ListOfAllSessionsAsync());
        }

        /// <summary>
        /// Remove one session by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("session-delete")]
        public async Task<ActionResult<string>> DeleteSessionById(int id)
        {
            return Ok(await this._service.RemoveOneSessionById(id));
        }

        /// <summary>
        /// Verify RBA
        /// </summary>
        /// <param name="k892"></param>
        /// <returns></returns>
        [HttpGet("lskda_2312sd2000123sdaSD")]
        public async Task<ActionResult<string>> RBAVerify(string k892)
        {
            return Ok(await this._service.VerifySession(k892));
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
        /// 2FA Code Generation
        /// </summary>
        /// <returns></returns>
        [HttpGet("2faC@d363n3r4t3")]
        public async Task<ActionResult<string>> GenerateTwoFACode()
        {
            return Ok(await this._service.TwoFactorAuthenticationCodeGeneration());
        }
        /// <summary>
        /// Change Email Adress
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPatch("r3f1orm@2-ema1l@213")]
        public async Task<ActionResult<string>> ReformEmailAddres([FromBody] NewEmailDTO body)
        {
            return Ok(await this._service.ChangeAddressEmail(body));
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPatch("upd4t3-p455w@rd")]
        public async Task<ActionResult<string>> UpdatePassword([FromBody] UpdatePasswordDTO body)
        {
            return Ok(await this._service.ChangePassword(body));
        }

        /// <summary>
        /// Remove Own Account
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("remove_ownaccount")]
        public async Task<ActionResult<string>> RemoveOwnAccountAsync([FromBody] RemoveOwnAccountDTO dto)
        {
            return Ok(await this._service.RemoveOwnAccount(dto));
        }
    }
}
