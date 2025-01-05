using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TP.Application.Common;
using TP.Domain.Constants;
using TP.Domain.Identity;

namespace TP.Application.Users;
public class VerifyRegisterUserCommand : IRequest<AuthResponseDTO>
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string RequestToken { get; set; }
    public string Code { get; set; }
}

public class VerifyRegisterUserCommandHandler : IRequestHandler<VerifyRegisterUserCommand, AuthResponseDTO>
{
    private readonly IJwtAuthService _tokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;
    private readonly IOtpService _otpService;

    public VerifyRegisterUserCommandHandler(IIdentityService identityService, IJwtAuthService tokenGenerator, UserManager<User> userManager, IOtpService otpService)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<AuthResponseDTO> Handle(VerifyRegisterUserCommand request, CancellationToken cancellationToken)
    {

        var OtpResult = _otpService.CheckOtp(new(request.UserName, request.RequestToken, request.Code));
        if (!OtpResult)
            throw new AppValidationException(Constants.OtpExpired);
        var user = new User()
        {
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            Email = request.UserName,
            Created = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.First().Description);
        }

        var addUserRole = await _userManager.AddToRolesAsync(user, Enumerable.Repeat(Roles.User, 1));
        if (!addUserRole.Succeeded)
        {
            throw new ValidationException(addUserRole.Errors.First().Description);
        }

        var userName = user.UserName;
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];
        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtAuth = _tokenGenerator.GenerateTokens(userName, claims.ToArray(), DateTime.UtcNow);

        return new AuthResponseDTO()
        {
            UserId = user.Id,
            Name = userName,
            Token = jwtAuth.AccessToken
        };
    }
}
