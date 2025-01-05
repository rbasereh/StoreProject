namespace TP.Domain.Contracts.Identity;

public class JwtAuthResult
{
    public string AccessToken { get; set; } = string.Empty;

    public RefreshToken RefreshToken { get; set; } = new();
}
