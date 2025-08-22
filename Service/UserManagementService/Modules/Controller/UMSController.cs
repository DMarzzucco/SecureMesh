using System;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Modules.Services.Interfaces;
using UserManagementService.Server.Users.Model;
using UserManagementService.Server.Users.Service.Interfaces;

namespace UserManagementService.Modules.Controller;

[Route("api/[controller]")]
[ApiController]
public class UMSController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IManagementUserServices _managementService;

    public UMSController(IUserService userService, IManagementUserServices managementService)
    {
        _userService = userService;
        _managementService = managementService;
    }

    /// <summary>
    /// Get All User Registered
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllListOfUser()
    {
        return Ok(await this._userService.ListOfAllUsers());
    }

    /// <summary>
    /// Get User Profile
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUserById(int id)
    {
        return Ok(await this._userService.GetUserProfile(id));
    }

    /// <summary>
    /// Update Own Registered
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPut("{id}/edit-registered")]
    public async Task<ActionResult<string>> EditOwnAccount(int id, [FromBody] UpdateOwnRegisterDTO body)
    {
        return Ok(await this._managementService.UpdateAnyCrendetial(id, body));
    }

    /// <summary>
    /// Update Roles
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPatch("{id}/u9d473-r0l35")]
    public async Task<ActionResult<string>> UpdateRoles(int id, [FromBody] RolesDTO body)
    {
        return Ok(await this._userService.UpdateUserRoles(id, body));
    }

    /// <summary>
    /// Remove any account
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("r3m0v3-4nn-4cc0yn7")]
    public async Task<ActionResult<string>> DeleteAnyAccount(int id)
    {
        return Ok(await this._userService.RemoveAnyAccount(id));
    }

    /// <summary>
    /// Verify new email
    /// </summary>
    /// <param name="klt1276"></param>
    /// <returns></returns>
    [HttpGet("{klt1276}/5413444_dsdn123fS_231_ddf")]
    public async Task<ActionResult<string>> ValidateNewEmail(string klt1276)
    {
        return Ok(await this._managementService.UpdateEmailAdress(klt1276));
    }

    /// <summary>
    /// ForgetPassword
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("elm23019_123mskw_123fnsk")]
    public async Task<ActionResult<string>> ForgetPassword([FromBody] ForgetPasswordDTO dto)
    {
        return Ok(await this._managementService.ForgetPasswordAccount(dto));
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
        return Ok(await this._managementService.ResetPassword(hmk12, body));
    }
}
