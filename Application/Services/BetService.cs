using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Application.Services;

public class BetService(IDbContextFactory<DatabaseContext> factory)
{
    public async Task<BetResultDto> BetAsync(BetPostDto form)
    {
        var result = new BetResultDto();
        await using var db = await factory.CreateDbContextAsync();
        //begin transaction
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            db.Bets.Add(new Bet()
            {
                /*public Guid UserGuid { get; set; }
    public Guid LivestreamGuid { get; set; }
    public int BetOnPlayer { get; set; }
    public DateTime BetDate { get; set; }
    public long PointsBet { get; set; }*/
            });
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
        }

        return result;
    }
}