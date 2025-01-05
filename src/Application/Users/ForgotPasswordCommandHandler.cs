using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using TP.Application.Common;
using TP.Domain.Identity;

namespace TP.Application.Users;
public record ForgotPasswordResponse(string RequestToken);
public record ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public string Email { get; set; }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IOtpService _otpService;
    private readonly IMemoryCache _cache;

    public ForgotPasswordCommandHandler(UserManager<User> userManager, IOtpService otpService, IMemoryCache cache)
    {
        _userManager = userManager;
        _otpService = otpService;
        _cache = cache;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var otpModel = _cache.Get<OtpModel>(CacheService.GetOTPKey(request.Email));
        if (otpModel != null)
            return new(otpModel.RequestToken);

        var user = await _userManager.FindByNameAsync(request.Email);
        if (user is null)
        {
            throw new AppValidationException("Email  not found");
        }


        var response = await _otpService.SendCode(new(request.Email));

        //todo:Send Email
        return new(response.RequestToken);
    }
}
