using Affiliate.Application.Database;
using Affiliate.Application.Events.Listeners;
using Affiliate.Application.Services.Auth;
using Spark.Library.Database;
using Coravel;
using Spark.Library.Auth;
using Affiliate.Application.Jobs;
using Spark.Library.Mail;
using Vite.AspNetCore.Extensions;
using FluentValidation;
using Affiliate.Pages.Auth;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Affiliate.Application.Startup;

public static class AppServicesRegistration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddViteServices();
        services.AddCustomServices();
        services.AddDatabase<DatabaseContext>(config);
        services.AddAuthorization(config, new string[]
        {
            CustomRoles.Master,
            CustomRoles.Admin,
            CustomRoles.Editor,
            CustomRoles.User
        });
        services.AddAuthentication<IAuthValidator>(config);
        services.AddJobServices();
        services.AddScheduler();
        services.AddQueue();
        services.AddEventServices();
        services.AddEvents();
        services.AddMailer(config);
        services.AddRazorComponents();
        services.AddDistributedMemoryCache();
        services.AddSession(options => {
            options.Cookie.Name = ".Affiliate";
            options.IdleTimeout = TimeSpan.FromMinutes(1);
        });
        
        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        // add custom services
        services.AddScoped<UserManageService>();
        services.AddScoped<UsersService>();
        services.AddScoped<RolesService>();
        services.AddScoped<IAuthValidator, SparkAuthValidator>();
        services.AddScoped<AuthService>();
        services.AddScoped<IValidator<Register.RegisterForm>, Register.RegisterFormValidator>();
        return services;
    }

    private static IServiceCollection AddEventServices(this IServiceCollection services)
    {
        // add custom events here
        services.AddTransient<EmailNewUser>();
        return services;
    }

    private static IServiceCollection AddJobServices(this IServiceCollection services)
    {
        // add custom background tasks here
        services.AddTransient<ExampleJob>();
        return services;
    }
}
