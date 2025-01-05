using TP.Application.Common.Interfaces;
using TP.Web.Services;
using Microsoft.AspNetCore.Mvc;
using TP.Application.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddSingleton<ILocalizerService, LocalizerService>();
        services.AddScoped<IUser, CurrentUser>();
        services.AddHttpContextAccessor();

        //services.AddHealthChecks()
        //    .AddDbContextCheck<ApplicationDbContext>();

        //services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        return services;
    }

}
