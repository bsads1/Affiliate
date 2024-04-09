using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class BetApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPost("/api/bet",
            async (BetPostDto form, BetService betService, LivestreamService livestreamService, HttpContext context) =>
            {
                var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                form.CreateBy = createBy;
                var live = await livestreamService.GetLiveAsync(form.Live);
                form.LiveGuid = live?.Guid ?? Guid.Empty;
                if (live.IsEnd)
                {
                    return Results.Ok(new
                    {
                        ok = false,
                        end = true,
                        messages = new[] { "This live is ended." }
                    });
                }

                var result = await betService.BetAsync(form);
                return Results.Ok(new
                {
                    ok = result.Success,
                    messages = result.Messages
                });
            }).RequireAuthorization();

        app.MapGet("/api/bet-history",
            async (int liveId, int? page, DatabaseContext dbContext) =>
            {
                page = page ?? 1;
                var history = (from bet in dbContext.Bets
                        join live in dbContext.Livestreams on bet.LivestreamGuid equals live.Guid
                        where live.Id == liveId
                        orderby bet.BetDate ascending
                        select new
                        {
                            bet.PointsBet,
                            bet.BetOnPlayer,
                            bet.RatioBet,
                            bet.RatioWon,
                            bet.Guid,
                            PointsWin = bet.PointsBet * (long)(bet.RatioWon / bet.RatioBet),
                            Date = bet.BetDate.ToString("dd/MM/yyyy")
                        })
                    .Skip((page.Value - 1) * 10)
                    .Take(50)
                    .ToList();
                return Results.Ok(new
                {
                    ok = true,
                    data = history
                });
            }).RequireAuthorization();

        app.MapGet("/api/all-bet-user-history", async (HttpContext context, DatabaseContext dbContext) =>
        {
            var user = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
            if (user == Guid.Empty) return Results.Unauthorized();
            var history = (from bet in dbContext.Bets
                    join live in dbContext.Livestreams on bet.LivestreamGuid equals live.Guid
                    where bet.UserGuid == user
                    orderby bet.BetDate ascending
                    select new
                    {
                        bet.PointsBet,
                        bet.BetOnPlayer,
                        bet.RatioBet,
                        bet.RatioWon,
                        bet.Guid,
                        live.IsEnd,
                        live.Winner,
                        pointsWin = bet.PointsBet * (long)(bet.RatioWon / bet.RatioBet),
                        win = live.Winner == bet.BetOnPlayer,
                        Date = bet.BetDate.ToString("dd/MM/yyyy")
                    })
                .ToList();
            return Results.Ok(new
            {
                ok = true,
                data = history
            });
        }).RequireAuthorization();

        app.MapPost("/api/bet-join", async (BetJoinForm form, BetService betService, HttpContext context) =>
        {
            var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
            form.UserId = createBy;
            var result = await betService.JoinBetAsync(form);
            return Results.Ok(new
            {
                ok = result != null
            });
        }).RequireAuthorization();

        app.MapGet("/api/bet-joins-list", async (DatabaseContext dbContext, HttpContext context) =>
        {
            var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
            var betJoins = (from bet in dbContext.Bets
                    join betjoin in dbContext.BetJoins on bet.Guid equals betjoin.BetGuid
                    where betjoin.UserGuid == createBy
                    select bet.Guid)
                .ToList();
            return Results.Ok(new
            {
                ok = true,
                data = betJoins
            });
        }).RequireAuthorization();
    }
}