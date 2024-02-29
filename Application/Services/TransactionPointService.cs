using Affiliate.Application.Database;
using Affiliate.Application.Enums;
using Affiliate.Application.Extensions.Paging;
using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Affiliate.Application.Services;

public class TransactionPointService(DatabaseContext context, IDbContextFactory<DatabaseContext> factory)
{
    public async Task<PaginatedList<TransactionPoint>> GetTransactionPoints(Guid userGuid, int page = 1,
        int pageSize = 10)
    {
        try
        {
            var queryable = context.TransactionPoints.AsNoTracking()
                .Where(p => p.UserId == userGuid)
                .OrderByDescending(p => p.TransactionTimestamp);
            return await queryable.ToPaginatedListAsync(page, pageSize);
        }
        catch (Exception e)
        {
            return [];
        }
    }

    public async Task<PaginatedList<TransactionPoint>> GetTransactionPoints(Guid userGuid, int page, int pageSize, string? filter, string? sort)
    {
        try
        {
            if(string.IsNullOrEmpty(filter))
                filter = "";
            var queryable = context.TransactionPoints.AsNoTracking()
                .Where(p => p.UserId == userGuid);
            if(!string.IsNullOrEmpty(filter))
                queryable = queryable.Where(p => p.TransactionType.ToLower() == filter.ToLower());
            if (!string.IsNullOrEmpty(sort))
            {
                queryable = sort switch
                {
                    "7days" => queryable.Where(p => p.TransactionTimestamp > DateTime.Now.AddDays(-7)),
                    "30days" => queryable.Where(p => p.TransactionTimestamp > DateTime.Now.AddDays(-30)),
                    "60days" => queryable.Where(p => p.TransactionTimestamp > DateTime.Now.AddDays(-60)),
                    "90days" => queryable.Where(p => p.TransactionTimestamp > DateTime.Now.AddDays(-90)),
                    "365days" => queryable.Where(p => p.TransactionTimestamp > DateTime.Now.AddYears(-1)),
                    _ => queryable
                };
            }
            return await queryable.OrderByDescending(p => p.TransactionTimestamp).ToPaginatedListAsync(page, pageSize);
        }
        catch (Exception e)
        {
            return [];
        }
    }

    public async Task<bool> AddPointAsync(int userId, long point, long money, string description, DateTimeOffset datetime,
        Guid byUser)
    {
        await using var db = await factory.CreateDbContextAsync();
        var transaction = await db.Database.BeginTransactionAsync();
        var useruid = "";
        try
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                Log.Error("Error add point to userid {UserId}: {Exception}", userId, "User not found");
                return false;
            }

            useruid = user.Uid;
            user.Points += point;

            var transactionPoint = new TransactionPoint
            {
                UserId = user.Guid,
                TransactionTimestamp = datetime.UtcDateTime,
                PointsChanged = point,
                TransactionType = TransactionType.Purchase.ToString(),
                TransactionDescription = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                CreatedBy = byUser,
                UpdatedBy = byUser,
                AccountBalance = user.Points,
                AmountChanged = money
            };
            db.TransactionPoints.Add(transactionPoint);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error add point to user {UserUid}: {Exception}", useruid, exception);
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> RemovePointAsync(int userId, long point, long money, string description, DateTimeOffset datetime,
        Guid byUser)
    {
        await using var db = await factory.CreateDbContextAsync();
        var transaction = await db.Database.BeginTransactionAsync();
        var useruid = "";
        try
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                Log.Error("Error minus point to userid {UserId}: {Exception}", userId, "User not found");
                return false;
            }

            useruid = user.Uid;
            user.Points += point;
            if (user.Points < 0) user.Points = 0;

            var transactionPoint = new TransactionPoint
            {
                UserId = user.Guid,
                TransactionTimestamp = datetime.UtcDateTime,
                PointsChanged = point,
                TransactionType = TransactionType.Adjustment.ToString(),
                TransactionDescription = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                CreatedBy = byUser,
                UpdatedBy = byUser,
                AccountBalance = user.Points,
                AmountChanged = money
            };
            db.TransactionPoints.Add(transactionPoint);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error minus point to user {UserUid}: {Exception}", useruid, exception);
            await transaction.RollbackAsync();
            return false;
        }
    }
}