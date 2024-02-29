using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class ConfigApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPost("/api/config",
            async (HttpContext context, ConfigService configService) =>
            {
                var pageName = context.Request.Form["PageName"][0];
                var description = context.Request.Form["Description"][0];
                var author = context.Request.Form["Author"][0];
                var keywords = context.Request.Form["Keywords"][0];
                var additionalMeta = context.Request.Form["AdditionalMeta"][0];
                var customCss = context.Request.Form["CustomCss"][0];
                var customJs = context.Request.Form["CustomJs"][0];
                (bool HasData, string Path) logoData = (false, "");
                (bool HasData, string Path) faviconData = (false, "");
                
                if (context.Request.Form.Files.Count > 0)
                {
                    var logo = context.Request.Form.Files["FileLogo"];
                    var favicon = context.Request.Form.Files["FileFavicon"];

                    //save logo and favicon
                    //check file logo
                    if (logo is { Length: > 0 } && !string.IsNullOrWhiteSpace(logo.FileName))
                    {
                        try
                        {
                            var logoPath = Path.Combine("wwwroot", "images", logo.FileName);
                            await using var stream = new FileStream(logoPath, FileMode.Create);
                            await logo.CopyToAsync(stream);
                            logoData.HasData = true;
                            logoData.Path = logoPath.GetRelativePath();
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    //check file favicon
                    if (favicon is { Length: > 0 } && !string.IsNullOrWhiteSpace(favicon.FileName))
                    {
                        try
                        {
                            var faviconPath = Path.Combine("wwwroot", "images", favicon.FileName);
                            await using var stream = new FileStream(faviconPath, FileMode.Create);
                            await favicon.CopyToAsync(stream);
                            faviconData.HasData = true;
                            faviconData.Path = faviconPath.GetRelativePath();
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }

                var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                var result = await configService.UpdateConfig(
                    new ConfigPostFormDto
                    {
                        PageName = pageName,
                        Description = description,
                        Author = author,
                        Keywords = keywords,
                        AdditionalMeta = additionalMeta,
                        CustomCss = customCss,
                        CustomJs = customJs,
                        Logo = logoData.Path,
                        Favicon = faviconData.Path,
                        LogoHasData = logoData.HasData,
                        FaviconHasData = faviconData.HasData,
                        CreatedBy = createBy
                    });

                if (result != null)
                {
                    return Results.Ok(new
                    {
                        Ok = true
                    });
                }
                return Results.Problem("Update config failed");
            })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);
    }
}