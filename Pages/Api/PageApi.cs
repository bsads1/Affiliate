using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Spark.Library.Extensions;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class PageApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapDelete("/api/page", async (HttpContext context, PageService pageService) =>
            {
                try
                {
                    var isParsed = int.TryParse(context.Request.Form["Id"][0], out var id);
                    if (isParsed)
                    {
                        var createBy =
                            Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                        var result = await pageService.DeletePageAsync(id, createBy);
                        if (result != null)
                        {
                            return Results.Ok(new
                            {
                                Ok = true
                            });
                        }
                    }

                    return Results.Ok(new
                    {
                        Ok = false
                    });
                }
                catch (Exception e)
                {
                    return Results.Problem(e.Message);
                }
            })
            .DisableAntiforgery()
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPost("/api/page",
                async ([FromForm] PagePostFormDto form, HttpContext context, PageService pageService) =>
                {
                    if (string.IsNullOrEmpty(form.Name))
                    {
                        return Results.Json(new
                        {
                            ok = false,
                            message = "Name is required"
                        });
                    }
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                    (bool HasData, string Path) imageData = (false, "");
                    (bool HasData, string Path) bannerData = (false, "");

                    var now = DateTime.Now;
                    var uploadPath = "wwwroot/uploads/" + now.Date.Year + "/" + now.Date.Month + "/" + now.Date.Day;
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    if (form.File is { Length: > 0 })
                    {
                        try
                        {
                            var filePath = Path.Combine(uploadPath, form.File.FileName);
                            await using var stream = new FileStream(filePath, FileMode.Create);
                            await form.File.CopyToAsync(stream);
                            imageData.HasData = true;
                            imageData.Path = filePath.GetRelativePath();
                        }
                        catch (Exception e)
                        {
                            imageData.HasData = false;
                        }
                    }

                    if (form.FileBanner.Check())
                    {
                        bannerData = await form.FileBanner.SaveAsync();
                    } 

                    if(string.IsNullOrWhiteSpace(form.Slug))
                        form.Slug = form.Name.ToSlug();
                    
                    form.CreatedBy = createBy;
                    form.HasImage = imageData.HasData;
                    form.HasBanner = bannerData.HasData;
                    form.Image = imageData.Path;
                    form.Banner = bannerData.Path;

                    var result = await pageService.CreatePageAsync(form);

                    if (result is { Id: > 0 })
                    {
                        return Results.Json(new
                        {
                            ok = true,
                            id = result.Id
                        });
                    }

                    return Results.Json(new
                    {
                        ok = false
                    });
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPut("/api/page",
                async ([FromForm] PagePostFormDto form, HttpContext context, PageService pageService) =>
                {
                    if (form.Id > 0)
                    {
                        if (string.IsNullOrEmpty(form.Name))
                        {
                            return Results.Json(new
                            {
                                ok = false,
                                message = "Name is required"
                            });
                        }
                        var createBy =
                            Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                        (bool HasData, string Path) imageData = (false, "");
                        (bool HasData, string Path) bannerData = (false, "");

                        var now = DateTime.Now;
                        var uploadPath = "wwwroot/uploads/" + now.Date.Year + "/" + now.Date.Month + "/" + now.Date.Day;
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }
                        if (form.File is { Length: > 0 })
                        {
                            try
                            {
                                var filePath = Path.Combine(uploadPath, form.File.FileName);
                                await using var stream = new FileStream(filePath, FileMode.Create);
                                await form.File.CopyToAsync(stream);
                                imageData.HasData = true;
                                imageData.Path = filePath.GetRelativePath();
                            }
                            catch (Exception e)
                            {
                                imageData.HasData = false;
                            }
                        }

                        if (form.FileBanner is { Length: > 0 })
                        {
                            try
                            {
                                var filePath = Path.Combine(uploadPath, form.FileBanner.FileName);
                                await using var stream = new FileStream(filePath, FileMode.Create);
                                await form.FileBanner.CopyToAsync(stream);
                                bannerData.HasData = true;
                                bannerData.Path = filePath.GetRelativePath();
                            }
                            catch (Exception e)
                            {
                                bannerData.HasData = false;
                            }
                        }

                        if(string.IsNullOrWhiteSpace(form.Slug))
                            form.Slug = form.Name.ToSlug();
                        form.UpdatedBy = createBy;
                        form.HasImage = imageData.HasData;
                        form.HasBanner = bannerData.HasData;
                        form.Image = imageData.Path;
                        form.Banner = bannerData.Path;

                        var result = await pageService.UpdatePageAsync(form);
                        if (result != null)
                        {
                            return Results.Json(new
                            {
                                ok = true
                            });
                        }
                    }

                    return Results.Json(new
                    {
                        ok = false
                    });
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);
    }
}