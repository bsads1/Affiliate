using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class SeoApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPost("/api/seo",
            async ([FromForm] PageSeoPostFormDto form, PageSeoService pageSeoService, HttpContext context) =>
            {
                (bool HasData, string Path) imageData = (false, "");

                if (form.File.Check())
                {
                    imageData = await form.File.SaveAsync();
                }

                form.HasImage = imageData.HasData;
                form.Image = imageData.Path;
                var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                form.CreatedBy = createBy;
                form.UpdatedBy = createBy;
                var result = await pageSeoService.CreateOrUpdateSeoAsync(form);
                if (result is { Id: > 0 })
                {
                    return Results.Json(new
                    {
                        ok = true,
                        id = result.Id
                    });
                }

                if (imageData.HasData)
                {
                    try
                    {
                        File.Delete(imageData.Path);
                    }
                    catch
                    {
                        //ignore
                    }
                }

                return Results.Json(new
                {
                    ok = false,
                    message = result.Error
                });
            });
    }
}