using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TP.Application.Common.Interfaces;
using TP.Domain.Common;
using TP.Domain.Identity;
using TP.Domain.UserSettings;
namespace TP.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    , IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<LocaleStringResource> LocaleStringResource { get; set; }
    public DbSet<MessageTemplate> MessageTemplate { get; set; }
    public DbSet<City> City { get; set; }
    public DbSet<Province> Province { get; set; }
}
