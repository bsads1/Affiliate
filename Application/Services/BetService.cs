using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Affiliate.Application.Services;

public class BetService(IDbContextFactory<DatabaseContext> factory, IHubContext<BetHub> hubContext)
{
    public async Task<BetResultDto> BetAsync(BetPostDto form)
    {
        var result = new BetResultDto();
        await using var db = await factory.CreateDbContextAsync();
        var messages = new List<string>();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var error = false;
        try
        {
            if (!(form.Bet >= 0))
            {
                messages.Add("Invalid bet");
                error = true;
            }

            if (form.Amount <= 0)
            {
                messages.Add("Invalid amount");
                error = true;
            }

            if (error)
            {
                result.Messages = messages;
                return result;
            }

            var user = await db.Users.FirstOrDefaultAsync(p => p.Guid == form.CreateBy);
            if (user != null)
            {
                if (user.Points < form.Amount)
                {
                    result.Messages.Add("Not enough point");
                    return result;
                }

                user.Points = user.Points - form.Amount;
                db.Users.Update(user);
                db.Bets.Add(new Bet
                {
                    UserGuid = form.CreateBy,
                    LivestreamGuid = form.LiveGuid,
                    BetOnPlayer = form.Bet,
                    BetDate = DateTime.Now.ToUniversalTime(),
                    PointsBet = form.Amount,
                    Guid = GuidExtension.TaoGuid(),
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    CreatedBy = form.CreateBy
                });

                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                result.Success = true;
            }
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error on bet: {Exception}", exception);
            await transaction.RollbackAsync();
            result.Messages = [exception];
            result.Success = false;
        }

        return result;
    }

    public async Task<BetResultDto> BetAsync(PostData? form)
    {
        var result = new BetResultDto();
        await using var db = await factory.CreateDbContextAsync();
        var messages = new List<string>();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var error = false;
        try
        {
            if (!(form.Bet >= 0))
            {
                messages.Add("Invalid bet");
                error = true;
            }

            if (form.PointBet <= 0)
            {
                messages.Add("Invalid amount");
                error = true;
            }

            if (error)
            {
                result.Messages = messages;
                return result;
            }

            var user = await db.Users.FirstOrDefaultAsync(p => p.Guid == form.CreateBy);
            if (user != null)
            {
                if (user.Points < form.PointBet)
                {
                    result.Messages.Add("Not enough point");
                    return result;
                }

                user.Points = user.Points - form.PointBet;
                db.Users.Update(user);
                var bet = new Bet
                {
                    UserGuid = form.CreateBy,
                    LivestreamGuid = form.LiveGuid,
                    BetOnPlayer = form.Bet,
                    BetDate = DateTime.Now.ToUniversalTime(),
                    PointsBet = form.PointBet,
                    RatioBet = form.RatioBet,
                    RatioWon = form.RatioWon,
                    Guid = GuidExtension.TaoGuid(),
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    CreatedBy = form.CreateBy
                };
                db.Bets.Add(bet);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                result.Success = true;
                result.Data = bet;
            }
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error on bet: {Exception}", exception);
            await transaction.RollbackAsync();
            result.Messages = [exception];
            result.Success = false;
        }

        return result;
    }

    public async Task<BetJoin> JoinBetAsync(BetJoinForm form)
    {
        await using var db = await factory.CreateDbContextAsync();

        var bet = await db.Bets.FirstOrDefaultAsync(p => p.Guid == form.BetGuid);
        if (bet == null || (bet.UserOpponentGuid != null && bet.UserOpponentGuid != Guid.Empty) || bet.IsPaid)
        {
            return null;
        }

        var user = await db.Users.FirstOrDefaultAsync(p => p.Guid == form.UserId);
        if (user == null)
        {
            return null;
        }

        var points = bet.PointsBet * (long)(bet.RatioWon / bet.RatioBet);

        if (user.Points < points)
        {
            return null;
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            if (user.Points < points)
            {
                return null;
            }

            var betJoin = new BetJoin
            {
                BetGuid = form.BetGuid,
                UserGuid = form.UserId,
                LivestreamGuid = bet.LivestreamGuid,
                JoinDate = DateTime.Now.ToUniversalTime(),
                BetOnPlayer = bet.BetOnPlayer == 1 ? 2 : 1,
                PointsBet = points,
                RatioBet = bet.RatioBet,
                RatioWon = bet.RatioWon
            };
            db.BetJoins.Add(betJoin);
            bet.UserOpponentGuid = form.UserId;
            user.Points = user.Points - points;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            await hubContext.Clients.All.SendAsync("UpdatePoint", user.Points);
            await hubContext.Clients.All.SendAsync("UpdateBet", bet.Id);

            return betJoin;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return null;
        }
    }
}