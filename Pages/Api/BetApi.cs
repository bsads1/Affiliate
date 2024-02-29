using Affiliate.Application.Dtos;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class BetApi: IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPost("/api/bet", async (BetPostDto form, BetService betService, LivestreamService livestreamService, HttpContext context) =>
        {
            var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
            form.CreateBy = createBy;
            var live = await livestreamService.GetLiveAsync(form.Live);
            form.LiveGuid = live?.Guid ?? Guid.Empty;
            var result = await betService.BetAsync(form);
            return Results.Ok(form);
        });
    }
}