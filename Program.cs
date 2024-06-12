using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Affiliate.Application.Services.Hubs;
using Serilog;
using Spark.Library.Config;
using Spark.Library.Environment;
using Spark.Library.Routing;
using Affiliate.Application.Startup;
using Affiliate.Pages;
using Microsoft.AspNetCore.ResponseCompression;
using Vite.AspNetCore;

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
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
builder.Services.AddSignalR();
var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}*/

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
app.UseResponseCompression();
app.MapRazorComponents<PageRoutes>().AddInteractiveServerRenderMode();
app.MapMinimalApis<Program>();
app.MapHub<BetHub>("/bethub");
app.Services.SetupScheduledJobs();
app.Services.SetupEvents();

app.Run();