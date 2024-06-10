using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiAccountMVP.Auth;
using MultiAccountMVP.Models;

namespace MultiAccountMVP;

public static class Program
{
    public static void AddMyAuth(this IServiceCollection services)
    {
        void AccountIdentityOptionsSetup(IdentityOptions options)
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        }

        void DevAccountIdentityOptionsSetup(IdentityOptions options)
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        }

        services.AddIdentity<Account, RoleModel>(AccountIdentityOptionsSetup)
            .AddRoles<RoleModel>()
            .AddEntityFrameworkStores<MainDbContext>()
            .AddSignInManager();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        });
        
        services.AddIdentityCore<DevAccount>(DevAccountIdentityOptionsSetup)
            .AddRoles<RoleModel>()
            .AddEntityFrameworkStores<MainDbContext>()
            .AddSignInManager();
        
        services.AddAuthentication()
            .AddCookie(CustomAuthSchemes.CookieDevAccount, options =>
            {
                options.Cookie.Name = CustomAuthSchemes.CookieDevAccount;
                options.Cookie.HttpOnly = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

        services.AddSingleton<DevRequirementHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(CustomAuthPolicies.DevAccount, policy =>
            {
                policy.AddRequirements(new DevRequirement());
                policy.AddAuthenticationSchemes(CustomAuthSchemes.CookieDevAccount);
                policy.RequireAuthenticatedUser();
            });
        });
    }
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMyAuth();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<MainDbContext>(options => options.UseInMemoryDatabase("MainDb"));

        builder.Services.AddControllers();
        
        var app = builder.Build();


        app.UseSwagger();
        app.UseSwaggerUI();


        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}