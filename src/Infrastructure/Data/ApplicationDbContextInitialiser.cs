using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TP.Domain.Common;
using TP.Domain.Constants;
using TP.Domain.Identity;

namespace TP.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly AppSettings _appSettings;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, AppSettings appSettings)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _appSettings = appSettings;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        //var xx = await _context.Set<TradeLog>().Where(e => e.CloseTime.HasValue).ToListAsync();
        //foreach (var t in xx)
        //{

        //    t.Duration = t.CloseTime.Value - t.OpenTime.Value;
        //}
        //await _context.SaveChangesAsync();


        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new Role() { Name = Roles.Admin });
            await _roleManager.CreateAsync(new Role() { Name = Roles.User });
        }

        if (!_userManager.Users.Any())
        {
            var adminUser = new User { UserName = "admin", Email = "admin@TP.com", SecurityStamp = Guid.NewGuid().ToString() };
            var x2 = await _userManager.CreateAsync(adminUser, "Aa@123456");
            var x = await _context.SaveChangesAsync();
            await _userManager.AddToRolesAsync(adminUser, [Roles.Admin]);
            await _context.SaveChangesAsync();
        }

    }
}
