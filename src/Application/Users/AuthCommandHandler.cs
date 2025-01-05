using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TP.Domain.Identity;

namespace TP.Application.Users;
public class AuthCommand : IRequest<AuthResponseDTO>
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class AuthResponseDTO
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
}

public class AuthCommandHandler : IRequestHandler<AuthCommand, AuthResponseDTO>
{
    private readonly IJwtAuthService _tokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;

    public AuthCommandHandler(IIdentityService identityService, IJwtAuthService tokenGenerator, UserManager<User> userManager)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
    }

    public async Task<AuthResponseDTO> Handle(AuthCommand request, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null)
        {
            throw new AppValidationException(Constants.RecordNotFound, nameof(User));
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
        {
            throw new AppValidationException(Constants.UserNameOrPasswordIsInvalid);
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
