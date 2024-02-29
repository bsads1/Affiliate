using System.Globalization;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class PointApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPut("/api/point",
            async (HttpContext context, TransactionPointService transactionPointService) =>
            {
                var id = int.TryParse(context.Request.Form["Id"][0], out var i) ? i : 0;
                var points = long.TryParse(context.Request.Form["Points"][0]?.Replace(".", ""), out var p) ? p : 0;
                var description = context.Request.Form["Description"][0] ?? "";
                var money = long.TryParse(context.Request.Form["Money"][0]?.Replace(".", ""), out var m) ? m : 0;
                var datetime = DateTimeOffset.TryParseExact(context.Request.Form["Datetime"][0], "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                    ? dt
                    : DateTime.UtcNow;
                var byUser = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                var result = points switch
                {
                    < 0 => await transactionPointService.RemovePointAsync(id, points, money, description, datetime,
                        byUser),
                    > 0 => await transactionPointService.AddPointAsync(id, points, money, description, datetime,
                        byUser),
                    _ => false
                };

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