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
                var ratio = 0;
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
                }

                var allBets = await db.Bets.AsNoTracking().Where(p => p.LivestreamGuid == livestream.Guid)
                    .ToListAsync();
                var winningBets = allBets.Where(p => p.BetOnPlayer == winner).ToList();
                if (winningBets.Count > 0)
                {
                    var userInBets = (from bet in winningBets
                        join u in db.Users.AsNoTracking() on bet.UserGuid equals u.Guid
                        select u).Distinct().ToList();
                    foreach (var item in winningBets)
                    {
                        var user = userInBets.FirstOrDefault(p => p.Guid == item.UserGuid);
                        var points = (long) (item.PointsBet * (item.RatioWon/item.RatioBet));
                        if (user != null)
                        {
                            user.Points += points;
                            db.Users.Update(user);
                            var transactionPoint = new TransactionPoint
                            {
                                UserId = user.Guid,
                                TransactionTimestamp = DateTime.Now.ToUniversalTime(),
                                PointsChanged = points,
                                TransactionType = TransactionType.Redemption.ToString(),
                                TransactionDescription = "Winning bets on " + livestream.Title,
                                CreatedAt = DateTime.Now.ToUniversalTime(),
                                UpdatedAt = DateTime.Now.ToUniversalTime(),
                                Guid = Guid.NewGuid(),
                                CreatedBy = createBy,
                                UpdatedBy = createBy,
                                AccountBalance = user.Points,
                                AmountChanged = 0
                            };
                            db.TransactionPoints.Add(transactionPoint);
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