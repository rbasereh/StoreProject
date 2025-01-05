using Microsoft.AspNetCore.Identity;
using TP.Application.Common;
using TP.Domain.Identity;

namespace TP.Application.Users;

public record RegisterUserResponse(string RequestToken);
public class RegisterUserCommand : IRequest<RegisterUserResponse>
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IJwtAuthService _tokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;
    private readonly IOtpService _otpService;

    public RegisterUserCommandHandler(IIdentityService identityService, IJwtAuthService tokenGenerator, UserManager<User> userManager, IOtpService otpService)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existUser = _userManager.Users.Any(e => e.UserName == request.UserName);
        if (existUser)
            throw new AppValidationException(Constants.RecordIsDuplicated, nameof(User));
        var otpResult = await _otpService.SendCode(new SendCodeModel(request.UserName));

        return new(otpResult.RequestToken);
    }
}
