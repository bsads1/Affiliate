using System.Runtime.CompilerServices;
using Affiliate.Application.Database;
using Affiliate.Application.Events.Listeners;
using Affiliate.Application.Extensions;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Affiliate.Application.Startup;

public static class AppServicesRegistration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddViteServices(options =>
        {
            options.Server.AutoRun = true;
            options.Server.Https = true;
        });
        services.AddCustomServices();

        services.AddDbContextFactory<DatabaseContext>(options =>
            options.UseNpgsql(GetConnectionString(config)).UseSnakeCaseNamingConvention());

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
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddDistributedMemoryCache();
        services.AddHttpContextAccessor();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".Affiliate";
            options.IdleTimeout = TimeSpan.FromMinutes(1);
        });
        services.AddFusionCache().WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            Duration = TimeSpan.FromMinutes(2)
        });

        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        // add custom services
        services.AddScoped<UserHelper>();
        services.AddScoped<TransactionPointService>();
        services.AddScoped<BetService>();
        services.AddScoped<MenuService>();
        services.AddScoped<PageService>();
        services.AddScoped<UserManageService>();
        services.AddScoped<ConfigService>();
        services.AddScoped<UsersService>();
        services.AddScoped<RolesService>();
        services.AddScoped<LivestreamService>();
        services.AddScoped<OrderPointService>();
        services.AddScoped<PageSeoService>();
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

    private static string GetConnectionString(IConfiguration config)
    {
        var str6 = config.GetValue<string>("Spark:Database:Drivers:Postgres:Host");
        var str7 = config.GetValue<string>("Spark:Database:Drivers:Postgres:Port");
        var str8 = config.GetValue<string>("Spark:Database:Drivers:Postgres:Database");
        var str9 = config.GetValue<string>("Spark:Database:Drivers:Postgres:Username");
        var str10 = config.GetValue<string>("Spark:Database:Drivers:Postgres:Password");
        var interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 5);
        interpolatedStringHandler2.AppendLiteral("Server=");
        interpolatedStringHandler2.AppendFormatted(str6);
        interpolatedStringHandler2.AppendLiteral(";Port=");
        interpolatedStringHandler2.AppendFormatted(str7);
        interpolatedStringHandler2.AppendLiteral(";Database=");
        interpolatedStringHandler2.AppendFormatted(str8);
        interpolatedStringHandler2.AppendLiteral(";Username=");
        interpolatedStringHandler2.AppendFormatted(str9);
        interpolatedStringHandler2.AppendLiteral(";Password=");
        interpolatedStringHandler2.AppendFormatted(str10);
        interpolatedStringHandler2.AppendLiteral(";");
        var connectionString = interpolatedStringHandler2.ToStringAndClear();
        return connectionString;
    }
}