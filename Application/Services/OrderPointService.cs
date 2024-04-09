using Affiliate.Application.Database;
using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Application.Services;

public class OrderPointService(IDbContextFactory<DatabaseContext> factory)
{
    public async Task<bool> CreateOrderAsync(OrderPoint form)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync();
            context.OrderPoints.Add(form);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<List<OrderPoint>> GetOrderListAsync(Guid userGuid)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync();
            var orderList = await context.OrderPoints
                .Where(x => x.UserGuid == userGuid && x.IsDelete == false)
                .OrderByDescending(p => p.CreatedAt).ToListAsync();
            return orderList;
        }
        catch (Exception e)
        {
            return new List<OrderPoint>();
        }
    }

    public async Task<bool> CreateWithdrawAsync(Withdraw form)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync();
            context.Withdraws.Add(form);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<List<Withdraw>> GetWithdrawListAsync(Guid userGuid)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync();
            var orderList = await context.Withdraws
                .Where(x => x.UserGuid == userGuid)
                .OrderByDescending(p => p.CreatedAt).ToListAsync();
            return orderList;
        }
        catch (Exception e)
        {
            return new List<Withdraw>();
        }
    }
}