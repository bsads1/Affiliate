using System.Globalization;
using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Spark.Library.Extensions;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class LivestreamApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapDelete("/api/live", async (HttpContext context, LivestreamService livestreamService) =>
            {
                try
                {
                    var isParsed = int.TryParse(context.Request.Form["Id"][0], out var id);
                    if (isParsed)
                    {
                        var createBy =
                            Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                        var result = await livestreamService.DeleteAsync(id, createBy);
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

        app.MapPost("/api/live",
                async ([FromForm] LivePostFormDto form, HttpContext context, LivestreamService livestreamService) =>
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                    if (string.IsNullOrEmpty(form.Title))
                    {
                        return Results.Json(new
                        {
                            ok = false,
                            message = "Title is required"
                        });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(form.StartTimeStr))
                        {
                            form.StartTime = DateTimeOffset.TryParseExact(form.StartTimeStr, "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : DateTime.UtcNow;
                        }

                        if (!string.IsNullOrEmpty(form.CloseTimeStr))
                        {
                            form.CloseTime = DateTimeOffset.TryParseExact(form.CloseTimeStr, "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : null;
                        }

                        if (!string.IsNullOrEmpty(form.AvailableTimeStartStr))
                        {
                            form.AvailableTimeStart = DateTimeOffset.TryParseExact(form.AvailableTimeStartStr,
                                "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : null;
                        }

                        if (!string.IsNullOrEmpty(form.AvailableTimeEndStr))
                        {
                            form.AvailableTimeEnd = DateTimeOffset.TryParseExact(form.AvailableTimeEndStr,
                                "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : null;
                        }

                        (bool HasData, string Path) imageData = (false, "");

                        if (form.File is { Length: > 0 })
                        {
                            try
                            {
                                var now = DateTime.Now;
                                var uploadPath = "wwwroot/uploads/" + now.Date.Year + "/" + now.Date.Month + "/" +
                                                 now.Date.Day;
                                if (!Directory.Exists(uploadPath))
                                {
                                    Directory.CreateDirectory(uploadPath);
                                }

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

                        form.Slug = form.Title.ToSlug();
                        form.CreatedBy = createBy;
                        form.CreatedAt = DateTime.UtcNow;
                        form.HasImage = imageData.HasData;
                        form.Image = imageData.Path;

                        var result = await livestreamService.CreateAsync(form);
                        return Results.Ok(new
                        {
                            ok = result != null, result?.Id
                        });
                    }
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPut("/api/live",
                async ([FromForm] LivePostFormDto form, HttpContext context, LivestreamService livestreamService) =>
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                    if (string.IsNullOrEmpty(form.Title))
                    {
                        return Results.Json(new
                        {
                            ok = false,
                            message = "Title is required"
                        });
                    }
                    else
                    {
                        (bool HasData, string Path) imageData = (false, "");

                        if (form.File is { Length: > 0 })
                        {
                            try
                            {
                                var now = DateTime.Now;
                                var uploadPath = "wwwroot/uploads/" + now.Date.Year + "/" + now.Date.Month + "/" +
                                                 now.Date.Day;
                                if (!Directory.Exists(uploadPath))
                                {
                                    Directory.CreateDirectory(uploadPath);
                                }

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

                        if (!string.IsNullOrEmpty(form.StartTimeStr))
                        {
                            form.StartTime = DateTimeOffset.TryParseExact(form.StartTimeStr, "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : DateTime.UtcNow;
                        }

                        if (!string.IsNullOrEmpty(form.CloseTimeStr))
                        {
                            form.CloseTime = DateTimeOffset.TryParseExact(form.CloseTimeStr, "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : null;
                        }

                        if (!string.IsNullOrEmpty(form.AvailableTimeStartStr))
                        {
                            form.AvailableTimeStart = DateTimeOffset.TryParseExact(form.AvailableTimeStartStr,
                                "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : null;
                        }

                        if (!string.IsNullOrEmpty(form.AvailableTimeEndStr))
                        {
                            form.AvailableTimeEnd = DateTimeOffset.TryParseExact(form.AvailableTimeEndStr,
                                "dd/MM/yyyy HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                                ? dt.UtcDateTime
                                : null;
                        }

                        form.Slug = form.Title.ToSlug();
                        form.HasImage = imageData.HasData;
                        form.Image = imageData.Path;
                        form.UpdatedAt = DateTime.UtcNow;
                        form.UpdatedBy = createBy;
                        var result = await livestreamService.UpdateAsync(form);
                        return Results.Ok(new
                        {
                            Ok = result != null, result?.Id
                        });
                    }
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPost("/api/live/update-winner/{live:int}",
                async (int live, [FromBody] PostForm form, HttpContext context,
                    LivestreamService livestreamService) =>
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                    var result = await livestreamService.UpdateWinnerAsync(live, form.winner, createBy);
                    return Results.Ok(new
                    {
                        Ok = result != null, result?.Winner
                    });
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);
    }

    private class PostForm
    {
        public int winner { get; set; }
    }
}