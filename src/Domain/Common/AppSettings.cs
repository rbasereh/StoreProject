using TP.Domain.Common.Identity;

namespace TP.Domain.Common;

public class AppSettings
{
    public JwtTokenConfig JwtTokenConfig { get; set; } = new();
    public int OtpTTl { get; set; }
    public string OtpStatic { get; set; }
    public MinioAppConfig MinioConfig { get; set; }
}
public class MinioAppConfig
{
    public string BaseUrl { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
}

