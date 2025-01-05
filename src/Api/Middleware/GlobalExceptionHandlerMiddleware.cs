using System.Net;
using Newtonsoft.Json;
using TP.Domain.Exceptions;
using TP.Web.Services;
namespace TP.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILocalizerService _localizerService;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILocalizerService localizerService)
    {
        _next = next;
        _localizerService = localizerService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppValidationException ex)
        {
            await HandleCustomExceptionAsync(context, ex);
        }
        catch (AppWarningException ex)
        {
            await HandleCustomExceptionAsync(context, ex, ex.StatusCode ?? 499);
        }
        catch (Exception ex)
        {
            await HandleUnhandledExceptionAsync(context, ex);
        }
    }

    private async Task HandleCustomExceptionAsync(HttpContext context, ApplicationCustomException exception, int? statusCode = null)
    {
        var lang = CurrentUser.GetLanguage(context);

        var key = string.Format(exception.Message, exception.Args);

        var localizeMessage = await _localizerService.GetAsync(key, lang, context.RequestAborted);

        await HandleExceptionAsync(context, localizeMessage, exception, statusCode);
    }
    private async Task HandleUnhandledExceptionAsync(HttpContext context, Exception exception, int? statusCode = null)
    {
        await HandleExceptionAsync(context, exception.Message, exception, statusCode);
    }
    private async Task HandleExceptionAsync(HttpContext context, string errorMessage, Exception exception, int? statusCode = null)
    {
        var response = new ApiResponse<string>
        {
            Success = false,
            Message = errorMessage,
            Data = null
        };

        context.Response.ContentType = "application/json";
        //todo: BadRequest for biz ex and 500 for server error
        context.Response.StatusCode = statusCode.HasValue ? statusCode.Value : (int)HttpStatusCode.BadRequest;

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}

