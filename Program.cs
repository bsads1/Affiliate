using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spark.Library.Config;
using Spark.Library.Environment;
using Spark.Library.Routing;
using Affiliate.Application.Database;
using Affiliate.Application.Startup;
using Affiliate.Pages;
using Vite.AspNetCore.Extensions;


EnvManager.LoadConfig();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetupAppConfig();
builder.Services.AddAppServices(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddWebEncoders(
    options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All)
);
//unicode range unicode


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseViteDevelopmentServer(true);
}

app.UseSerilogRequestLogging();
app.UseStatusCodePagesWithRedirects("/status-code/{0}");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseSession();
app.UseAntiforgery();
app.MapRazorComponents<PageRoutes>().AddInteractiveServerRenderMode();
app.MapMinimalApis<Program>();

app.Services.SetupScheduledJobs();
app.Services.SetupEvents();

app.Run();