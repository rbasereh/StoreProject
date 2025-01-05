using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace TP.Application.Common;

public class CacheService
{
    public static string GetLocalizeKey(string lang, string key) => $"res_{lang}_{key}";
    public static string GetOTPKey(string email) => $"otp_{email}";
}

public record SendCodeModel(string Email);
public record CheckOtpCodeModel(string Email, string RequestToken, string Code);
public record SendCodeResopnseModel(string RequestToken);
public record OtpModel(string RequestToken, string Code, List<Claim> Claims);
public interface IOtpService
{
    Task<SendCodeResopnseModel> SendCode(SendCodeModel request, List<Claim> claims = null);
    bool CheckOtp(CheckOtpCodeModel request);

}
public class OtpService : IOtpService
{
    private readonly IMemoryCache _cache;
    private readonly AppSettings _appSettings;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly EmailService _emailService;

    public OtpService(IMemoryCache cache, AppSettings appSettings, IHostEnvironment hostEnvironment, EmailService emailService)
    {
        _cache = cache;
        _appSettings = appSettings;
        _hostEnvironment = hostEnvironment;
        _emailService = emailService;
    }
    public async Task<SendCodeResopnseModel> SendCode(SendCodeModel request, List<Claim> claims = null)
    {
        //todo: validate Email
        if (string.IsNullOrEmpty(request.Email))
            throw new ArgumentNullException(nameof(request.Email));

        //Check if exist use same OtpCode
        var otpModel = _cache.Get<OtpModel>(CacheService.GetOTPKey(request.Email));
        if (otpModel != null)
            return new(otpModel.RequestToken);

        var otpCode = new Random().Next(100000, 999999).ToString();
        var requestToken = Guid.NewGuid().ToString();

        //todo: add request to DB (Redis)
        _cache.Set<OtpModel>(CacheService.GetOTPKey(request.Email), new(requestToken, otpCode, claims), TimeSpan.FromSeconds(_appSettings.OtpTTl));
        SendCodeResopnseModel response = new(requestToken);

        //send OtpCode with Email
        _emailService.Send(request.Email, otpCode);

        return response;
    }

    public bool CheckOtp(CheckOtpCodeModel request)
    {
        //todo: validate phoneNumber
        if (string.IsNullOrEmpty(request.Email))
            throw new AppValidationException(Constants.PropIsRequired, nameof(request.Email));

        //Check if exist use same OtpCode
        var otpModel = _cache.Get<OtpModel>(CacheService.GetOTPKey(request.Email));
        if (otpModel is null)
            //todo: code is expired
            throw new AppValidationException(Constants.OtpExpired);

        if (request.RequestToken != otpModel.RequestToken)
            throw new AppValidationException(Constants.OtpIsInvalid);

        if ((!_hostEnvironment.IsDevelopment() && request.Code != otpModel.Code)
            || (_hostEnvironment.IsDevelopment() && request.Code != otpModel.Code && request.Code != _appSettings.OtpStatic))
            throw new AppValidationException(Constants.OtpIsInvalid);

        return true;
    }
}
