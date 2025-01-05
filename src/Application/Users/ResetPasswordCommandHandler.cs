using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using TP.Application.Common;
using TP.Domain.Identity;

namespace TP.Application.Users;

public class ResetPasswordResponse
{

}

public record ResetPasswordCommand(string Email, string RequestToken, string Code, string NewPassword) : IRequest<ResetPasswordResponse>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IOtpService _otpService;
    private readonly IMemoryCache _cache;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly AppSettings _appSettings;


    public ResetPasswordCommandHandler(UserManager<User> userManager, IOtpService otpService, IMemoryCache cache, IHostEnvironment hostEnvironment, AppSettings appSettings)
    {
        _userManager = userManager;
        _otpService = otpService;
        _cache = cache;
        _hostEnvironment = hostEnvironment;
        _appSettings = appSettings;
    }

    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new AppValidationException(nameof(request));

        //todo: validate phoneNumber
        if (string.IsNullOrEmpty(request.Email))
            throw new AppValidationException(nameof(request.Email));

        //Check if exist use same OtpCode
        var otpModel = _cache.Get<OtpModel>(CacheService.GetOTPKey(request.Email));
        if (otpModel is null)
            //todo: code is expired
            throw new AppValidationException("Requset Expired");

        if (request.RequestToken != otpModel.RequestToken)
            throw new AppValidationException(nameof(request.RequestToken));

        if ((!_hostEnvironment.IsDevelopment() && request.Code != otpModel.Code)
            || (_hostEnvironment.IsDevelopment() && request.Code != otpModel.Code && request.Code != _appSettings.OtpStatic))
            throw new AppValidationException(nameof(request.Code));


        var user = await _userManager.FindByNameAsync(request.Email);

        var result = await _userManager.RemovePasswordAsync(user);
        result = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!result.Succeeded)
            throw new AppValidationException(result.Errors.First().Description);

        return new();
    }
}
