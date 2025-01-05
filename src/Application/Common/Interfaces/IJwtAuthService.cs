using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TP.Domain.Contracts.Identity;

namespace TP.Application.Common.Interfaces;

public interface IJwtAuthService
{
    IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }
    JwtAuthResult GenerateTokens(string userName, Claim[] claims, DateTime now);
    JwtAuthResult Refresh(string userName, string refreshToken, string accessToken, DateTime now);
    void RemoveExpiredRefreshTokens(DateTime now);
    void RemoveRefreshTokenByUserName(string userName);
    (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
}
