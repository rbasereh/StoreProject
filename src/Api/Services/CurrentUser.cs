using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using TP.Application.Common.Interfaces;

namespace TP.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public int UserId
    {
        get
        {
            int.TryParse(Id, out int userid);
            return userid == 0 ? 0 : userid;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier || x.Type == "sub")?.Value;

    public async Task<string?> GetTokenAsync()
       => await _httpContextAccessor.HttpContext?.GetTokenAsync("Bearer", "access_token");

    public Language Language => GetLanguage(_httpContextAccessor.HttpContext);
    public static Language GetLanguage(HttpContext httpContext)
    {
        var language = httpContext.Request.Headers["Lang"].ToString();
        if (string.IsNullOrWhiteSpace(language))
            language = "en";
        return language switch { "fa" => Language.Fa, "en" => Language.En, _ => Language.Fa };
    }

    public string UserAgent => _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;

    public string Code => string.Format("1{0:D7}", UserId);
}
