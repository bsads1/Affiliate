using Affiliate.Application.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Spark.Library.Auth;

namespace Affiliate.Application.Startup;

public static class AuthServiceRegistration
{
    public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration config,
        string[] roles)
    {
        services.AddAuthorization(options =>
        {
            foreach (var role in roles)
            {
                options.AddPolicy(role, policy => policy.RequireRole(role));
            }
            
            options.AddPolicy(CustomPolicies.AdminAccess, policy =>
                policy.RequireRole(CustomRoles.Master, CustomRoles.Admin, CustomRoles.Editor));

            options.AddPolicy(CustomPolicies.MasterAdminAccess, policy =>
                policy.RequireRole(CustomRoles.Master, CustomRoles.Admin));
            
            options.AddPolicy(CustomPolicies.UserAccess, policy =>
                policy.RequireAuthenticatedUser());
        });
        return services;
    }

    public static IServiceCollection AddAuthentication<T>(this IServiceCollection services, IConfiguration config)
        where T : IAuthValidator
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.SlidingExpiration = false;
                options.LoginPath = config.GetValue<string>("Spark:Auth:LoginPath", "/login");
                options.LogoutPath = config.GetValue<string>("Spark:Auth:LogoutPath", "/logout");
                options.AccessDeniedPath = config.GetValue<string>("Spark:Auth:AccessDeniedPath", "/access-denied");
                options.Cookie.Name = ".blazor.spark.cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        var cookieValidatorService = context.HttpContext.RequestServices.GetRequiredService<T>();
                        return cookieValidatorService.ValidateAsync(context);
                    }
                };
            });
        return services;
    }
}