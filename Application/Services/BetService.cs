using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Affiliate.Application.Services;

public class BetService(IDbContextFactory<DatabaseContext> factory)
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
            var user = await db.Users.FirstOrDefaultAsync(p=>p.Guid == form.CreateBy);
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
                    BetDate = DateTime.UtcNow,
                    PointsBet = form.Amount,
                    Guid = GuidExtension.TaoGuid(),
                    CreatedAt = DateTime.UtcNow,
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
}