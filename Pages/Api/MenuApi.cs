using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class MenuApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapGet("/api/menu",async (MenuService menuService) =>
        {
            var menus = await menuService.GetMenuItemsAsync();
            var response = menus.Select(p => new
            {
                id = p.Id,
                level = p.Level,
                parent_id = p.ParentId,
                title = p.Title,
                link = p.Link
            }).ToList();
            return Results.Json(response); 
        })
        .RequireAuthorization(CustomPolicies.MasterAdminAccess);
        
        app.MapPost("/api/menu",
            async (HttpContext context, MenuService menuService) =>
            {
                var id = int.TryParse(context.Request.Form["Id"], out var i) ? i : 0;
                var title = context.Request.Form["Title"][0] ?? "";
                var link = context.Request.Form["Link"][0] ?? "";
                var parentId = int.TryParse(context.Request.Form["ParentId"], out var p) ? p : 0;
                var level = int.TryParse(context.Request.Form["Level"], out var l) ? l : 0;
                var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                var result = await menuService.CreateAsync(id, title, link, parentId, level,createBy);

                var response = new
                {
                    ok = result,
                    message = result ? "Success" : "Failed"
                };
                return Results.Json(response);
            })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPut("/api/menu",
            async (HttpContext context, MenuService menuService) =>
            {
                var id = int.TryParse(context.Request.Form["Id"], out var i) ? i : 0;
                var title = context.Request.Form["Title"][0] ?? "";
                var link = context.Request.Form["Link"][0] ?? "";
                var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                var result = await menuService.UpdateAsync(id, title, link, createBy);

                var response = new
                {
                    ok = result,
                    message = result ? "Success" : "Failed"
                };
                return Results.Json(response);
            })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPut("/api/menu/update-position",
            async (HttpContext context, MenuService menuService) =>
            {
                var id = int.TryParse(context.Request.Form["Id"][0], out var i) ? i : 0;
                var parentId = int.TryParse(context.Request.Form["ParentId"][0], out var p) ? p : 0;
                var level = int.TryParse(context.Request.Form["Level"][0], out var l) ? l : 0;
                var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                var result = await menuService.UpdateAsync(id, parentId, level, createBy);

                var response = new
                {
                    ok = result,
                    message = result ? "Success" : "Failed"
                };
                return Results.Json(response);
            })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);
    }
}