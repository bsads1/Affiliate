using Affiliate.Application.Enums;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class TopupApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPost("/api/user/deposit", async (OrderPoint form, OrderPointService orderPointService, HttpContext context) =>
        {
            var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
            form.CreatedBy = createBy;
            form.UserGuid = createBy;
            form.CreatedAt = DateTime.Now.ToUniversalTime();
            form.IsDelete = false;
            form.Status = OrderPointStatus.Pending.ToString();
            var result = await orderPointService.CreateOrderAsync(form);
            return Results.Ok(new
            {
                ok = result
            });
        }).RequireAuthorization();
        
        app.MapPost("/api/user/withdraw", async (Withdraw form, OrderPointService orderPointService, HttpContext context) =>
        {
            var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
            form.CreatedBy = createBy;
            form.UserGuid = createBy;
            form.CreatedAt = DateTime.Now.ToUniversalTime();
            form.Status = OrderPointStatus.Pending.ToString();
            form.Note = "";
            var result = await orderPointService.CreateWithdrawAsync(form);
            return Results.Ok(new
            {
                ok = result
            });
        }).RequireAuthorization();
    }
}