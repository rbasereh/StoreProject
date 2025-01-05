using Microsoft.AspNetCore.Identity;
using TP.Application.Common;
using TP.Domain.Identity;

namespace TP.Application.Users;
public class VerifyOtpCodeCommand : IRequest<bool>
{
    public string UserName { get; set; }
    public string RequestToken { get; set; }
    public string Code { get; set; }
}

public class VerifyOtpCodeCommandHandler : IRequestHandler<VerifyOtpCodeCommand, bool>
{
    private readonly IJwtAuthService _tokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;
    private readonly IOtpService _otpService;

    public VerifyOtpCodeCommandHandler(IIdentityService identityService, IJwtAuthService tokenGenerator, UserManager<User> userManager, IOtpService otpService)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<bool> Handle(VerifyOtpCodeCommand request, CancellationToken cancellationToken)
    {

        var OtpResult = _otpService.CheckOtp(new(request.UserName, request.RequestToken, request.Code));
        if (!OtpResult)
            throw new AppValidationException(Constants.OtpExpired);
       
        return true;
    }
}
