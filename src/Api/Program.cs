
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using TP.Api.Middleware;
using TP.Application.Common.Interfaces;
using TP.Application.Common.Swagger;
using TP.Infrastructure;
using TP.Infrastructure.Common.Identity;
using TP.Infrastructure.Data;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddWebServices();
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMemoryCache();
        //builder.Services.AddSwaggerGen();


        var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
        builder.Services.AddSingleton(appSettings);
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = appSettings.JwtTokenConfig.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JwtTokenConfig.Secret)),
                ValidAudience = appSettings.JwtTokenConfig.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        builder.Services.AddSingleton<IJwtAuthService, JwtAuthService>();
        builder.Services.AddSingleton<EmailService>();
        
        builder.Services.AddHttpClient();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(ApiGroup.Admin, new OpenApiInfo { Title = ApiGroup.Admin, Version = "v1" });
            c.SwaggerDoc(ApiGroup.User, new OpenApiInfo { Title = ApiGroup.User, Version = "v1" });
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lowercase
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.SchemaFilter<SwaggerExcludeFilter>();
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
        });


        var app = builder.Build();
        await app.InitialiseDatabaseAsync();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseExchangeSwagger();
        }

        //app.UseHttpsRedirection();

        app.UseRouting();
        app.UseMiddleware<JwtMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

}
public static class WebApplicationExt
{
    public static WebApplication UseExchangeSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{ApiGroup.Admin}/swagger.json", $"{ApiGroup.Admin}");
            c.SwaggerEndpoint($"/swagger/{ApiGroup.User}/swagger.json", $"{ApiGroup.User}");
            c.DocExpansion(DocExpansion.None);
        });

        return app;
    }
}
public class ApiGroup
{
    public const string Admin = "TP.Admin";
    public const string User = "TP.User";

}
