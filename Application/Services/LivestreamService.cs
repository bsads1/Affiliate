using System.Text.Json;
using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Enums;
using Affiliate.Application.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Affiliate.Application.Services;

public class LivestreamService(IDbContextFactory<DatabaseContext> factory, DatabaseContext context)
{
    public async Task<Livestream?> DeleteAsync(int id, Guid updateBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var livestream = await db.Livestreams.FindAsync(id);
            if (livestream != null)
            {
                livestream.IsDelete = true;
                livestream.UpdatedAt = DateTime.Now.ToUniversalTime();
                livestream.UpdatedBy = updateBy;
                await db.SaveChangesAsync();
                return livestream;
            }

            return null;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return null;
        }
    }

    public async Task<Livestream?> CreateAsync(LivePostFormDto form)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var livestream = form.Adapt<Livestream>();
            livestream.Guid = Guid.NewGuid();
            await db.Livestreams.AddAsync(livestream);
            await db.SaveChangesAsync();
            return livestream;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
        }

        return null;
    }

    public async Task<Livestream?> UpdateAsync(LivePostFormDto form)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var livestream = await db.Livestreams.FindAsync(form.Id);
            if (livestream != null)
            {
                if (livestream.Guid == Guid.Empty)
                {
                    livestream.Guid = Guid.NewGuid();
                }

                livestream.UpdatedAt = form.UpdatedAt;
                livestream.UpdatedBy = form.UpdatedBy;
                livestream.Title = form.Title;
                livestream.Slug = form.Slug;
                livestream.Content = form.Content;
                livestream.LivestreamInput = form.LivestreamInput;
                livestream.StartTime = form.StartTime.ToUniversalTime();
                livestream.CloseTime = form.CloseTime?.ToUniversalTime();
                livestream.AvailableTimeStart = form.AvailableTimeStart?.ToUniversalTime();
                livestream.AvailableTimeEnd = form.AvailableTimeEnd?.ToUniversalTime();
                livestream.Player1Name = form.Player1Name;
                livestream.Player2Name = form.Player2Name;
                //.IsShow = form.IsShow;
                if (form.HasImage)
                {
                    livestream.Image = form.Image;
                }

                db.Livestreams.Update(livestream);
                await db.SaveChangesAsync();
                return livestream;
            }

            return null;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return null;
        }
    }

    public async Task<Livestream?> GetLiveAsync(int id)
    {
        var livestream = await context.Livestreams.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return livestream;
    }

    public async Task<List<BetDto>> GetLiveBets(Guid liveGuid)
    {
        var result = new List<BetDto>();
        try
        {
            var betsDto = await (from bet in context.Bets.AsNoTracking()
                join u in context.Users.AsNoTracking() on bet.UserGuid equals u.Guid
                where bet.LivestreamGuid == liveGuid
                orderby bet.BetDate descending
                select new BetDto
                {
                    Id = bet.Id,
                    UserName = u.Name,
                    BetDate = bet.BetDate,
                    BetOnPlayer = bet.BetOnPlayer,
                    PointsBet = bet.PointsBet,
                    CreatedAt = bet.CreatedAt,
                    UpdatedAt = bet.UpdatedAt
                }).ToListAsync();
            return betsDto;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error getting live bets: {Exception}", exception);
        }

        return result;
    }

    public async Task<Livestream?> UpdateWinnerAsync(int liveId, int winner, Guid createBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var livestream = await db.Livestreams.FindAsync(liveId);
            if (livestream != null)
            {
                livestream.Winner = winner;
                livestream.IsEnd = true;
                livestream.UpdatedAt = DateTime.Now.ToUniversalTime();
                livestream.UpdatedBy = createBy;
                await db.SaveChangesAsync();
                /*var ratio = 0;
                if (!string.IsNullOrEmpty(livestream.Ratio))
                {
                    var split = livestream.Ratio.Split(':');
                    if (split.Length == 2)
                    {
                        if (livestream.Winner == 1)
                        {
                            ratio = int.Parse(split[1]) / int.Parse(split[0]);
                        }
                        else if (livestream.Winner == 2)
                        {
                            ratio = int.Parse(split[0]) / int.Parse(split[1]);
                        }
                    }
                }*/

                var allBets = await db.Bets.AsNoTracking().Where(p => p.LivestreamGuid == livestream.Guid && !p.IsPaid)
                    .ToListAsync();
                /*var allJoinBets = (from jb in db.BetJoins
                    join u in db.Bets on jb.BetGuid equals u.Guid
                    where jb.LivestreamGuid == livestream.Guid && u.LivestreamGuid == livestream.Guid
                        select jb).AsNoTracking().OrderBy(p=>p.Id).ToList();*/
                //var winningBets = allBets.Where(p => p.BetOnPlayer == winner).ToList();
                if (allBets.Count > 0)
                {
                    var userInBets = (from bet in allBets
                        join u in db.Users.AsNoTracking() on bet.UserGuid equals u.Guid
                        select u).Distinct().ToList();
                    var userInJoinBets = (from bet in allBets
                        join u in db.Users.AsNoTracking() on bet.UserOpponentGuid equals u.Guid
                        select u).ToList();
                    /*var userInJoinBets = (from jb in allJoinBets
                        join u in db.Users.AsNoTracking() on jb.UserGuid equals u.Guid
                        select u).Distinct().ToList();*/

                    foreach (var item in allBets)
                    {
                        if(item.IsPaid) continue;
                        var user = userInBets.FirstOrDefault(p => p.Guid == item.UserGuid);
                        //var betJoins = allJoinBets.Where(p => p.BetGuid == item.Guid);
                        if (user != null)
                            if (item.UserOpponentGuid != null && item.UserOpponentGuid != Guid.Empty)
                            {
                                if (item.BetOnPlayer == winner)
                                {
                                    //user win, joins lose
                                    long points = item.PointsBet * (long)(item.RatioWon / item.RatioBet);
                                    user.Points += points;
                                    db.Users.Update(user);
                                    var transactionPoint = new TransactionPoint
                                    {
                                        UserId = user.Guid,
                                        TransactionTimestamp = DateTime.Now.ToUniversalTime(),
                                        PointsChanged = points,
                                        TransactionType = TransactionType.Redemption.ToString(),
                                        TransactionDescription = "Redemption bets on winning live " + livestream.Guid,
                                        CreatedAt = DateTime.Now.ToUniversalTime(),
                                        UpdatedAt = DateTime.Now.ToUniversalTime(),
                                        Guid = Guid.NewGuid(),
                                        CreatedBy = createBy,
                                        UpdatedBy = createBy,
                                        AccountBalance = user.Points,
                                        AmountChanged = 0
                                    };
                                    db.TransactionPoints.Add(transactionPoint);
                                    item.IsPaid = true;
                                }
                                else
                                {
                                    //user lose, joins win
                                    var userWin = userInJoinBets.FirstOrDefault(p => p.Guid == item.UserOpponentGuid);
                                    if (userWin != null)
                                    {
                                        long points = item.PointsBet;
                                        userWin.Points += points;
                                        db.Users.Update(userWin);
                                        var transactionPoint = new TransactionPoint
                                        {
                                            UserId = userWin.Guid,
                                            TransactionTimestamp = DateTime.Now.ToUniversalTime(),
                                            PointsChanged = points,
                                            TransactionType = TransactionType.Redemption.ToString(),
                                            TransactionDescription = "Redemption bets on winning live " + livestream.Guid,
                                            CreatedAt = DateTime.Now.ToUniversalTime(),
                                            UpdatedAt = DateTime.Now.ToUniversalTime(),
                                            Guid = Guid.NewGuid(),
                                            CreatedBy = createBy,
                                            UpdatedBy = createBy,
                                            AccountBalance = userWin.Points,
                                            AmountChanged = 0
                                        };
                                        db.TransactionPoints.Add(transactionPoint);
                                        item.IsPaid = true;
                                    }
                                }
                            }
                            else
                            {
                                //trả lại điểm cho user
                                //var points = (long) (item.PointsBet * (item.RatioWon/item.RatioBet));
                                if (user != null)
                                {
                                    user.Points += item.PointsBet;
                                    db.Users.Update(user);
                                    var transactionPoint = new TransactionPoint
                                    {
                                        UserId = user.Guid,
                                        TransactionTimestamp = DateTime.Now.ToUniversalTime(),
                                        PointsChanged = item.PointsBet,
                                        TransactionType = TransactionType.Refund.ToString(),
                                        TransactionDescription = "Refund bets on " + livestream.Guid +
                                                                 " live because no user joined the bet",
                                        CreatedAt = DateTime.Now.ToUniversalTime(),
                                        UpdatedAt = DateTime.Now.ToUniversalTime(),
                                        Guid = Guid.NewGuid(),
                                        CreatedBy = createBy,
                                        UpdatedBy = createBy,
                                        AccountBalance = user.Points,
                                        AmountChanged = 0
                                    };
                                    db.TransactionPoints.Add(transactionPoint);
                                    item.IsPaid = true;
                                }
                            }
                    }

                    await db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return livestream;
            }
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating winner: {Exception}", exception);
        }

        return null;
    }

    public async Task<bool> UpdatePublishAsync(int liveId, Guid createBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var livestreams = await db.Livestreams.ToListAsync();
            foreach (var livestream in livestreams)
            {
                if (livestream.Id == liveId)
                {
                    livestream.IsShow = true;
                    livestream.UpdatedAt = DateTime.Now.ToUniversalTime();
                    livestream.UpdatedBy = createBy;
                }
                else
                {
                    livestream.IsShow = false;
                    livestream.UpdatedAt = DateTime.Now.ToUniversalTime();
                    livestream.UpdatedBy = createBy;
                }

                db.Livestreams.Update(livestream);
            }

            await db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating publish: {Exception}", exception);
            return false;
        }
    }

    public Livestream? GetLiveAsync()
    {
        try
        {
            var livestreams = context.Livestreams.AsNoTracking().FirstOrDefault(p => p.IsShow == true);

            return livestreams;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}