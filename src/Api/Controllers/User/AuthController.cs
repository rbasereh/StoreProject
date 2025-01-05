using Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP.Application.Users;
namespace TP.Api.Controllers.Users;

[ApiController]
[Route("api/user/auth")]
[ApiExplorerSettings(GroupName = ApiGroup.User)]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ApiResponse<AuthResponseDTO>> Login([FromBody] AuthCommand command)
    {
        var data = await mediator.Send(command);
        return new() { Data = data };
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ApiResponse<RegisterUserResponse>> Register([FromBody] RegisterUserCommand command)
    {
        var data = await mediator.Send(command);
        return new() { Data = data };
    }

    [AllowAnonymous]
    [HttpPost("verify-register")]
    public async Task<ApiResponse<AuthResponseDTO>> VerifyRegister([FromBody] VerifyRegisterUserCommand command)
    {
        var data = await mediator.Send(command);
        return new() { Data = data };
    }

    [AllowAnonymous]
    [HttpPost("verify-code")]
    public async Task<ApiResponse<bool>> VerifyCode([FromBody] VerifyOtpCodeCommand command)
    {
        var data = await mediator.Send(command);
        return new() { Data = data };
    }
    
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<ApiResponse<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var data = await mediator.Send(command);
        return new() { Data = data };
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ApiResponse<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var data = await mediator.Send(command);
        return new() { Data = data };
    }
}
