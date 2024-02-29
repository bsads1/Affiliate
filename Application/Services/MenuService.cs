using Affiliate.Application.Database;
using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZiggyCreatures.Caching.Fusion;

namespace Affiliate.Application.Services;

public class MenuService(IDbContextFactory<DatabaseContext> factory, DatabaseContext context, IFusionCache cache)
{
    const string CacheKey = "Cache:MenuItems";

    public async Task<bool> CreateAsync(int id, string title, string link, int parentId, int level, Guid createBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var menu = new MenuItem
            {
                Title = title,
                Link = link,
                ParentId = parentId,
                Level = level,
                CreatedBy = createBy,
                CreatedAt = DateTime.UtcNow
            };
            db.MenuItems.Add(menu);
            var result = await db.SaveChangesAsync();
            if (result > 0)
            {
                await cache.RemoveAsync(CacheKey);
            }

            return result > 0;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error create menu: {Exception}", exception);
            return false;
        }
    }

    public async Task<List<MenuItem>> GetMenuItemsAsync()
    {
        try
        {
            async Task<List<MenuItem>> GetMenuItemsAsyncLocal()
            {
                var menus = await context.MenuItems
                    .AsNoTracking()
                    .OrderBy(p => p.Id)
                    .ToListAsync();
                return menus;
            }

            var menusCached = await cache.GetOrSetAsync<List<MenuItem>>(CacheKey,
                 async _ => await GetMenuItemsAsyncLocal(),
                TimeSpan.FromMinutes(5));
            return menusCached;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error get menu: {Exception}", exception);
            return new List<MenuItem>();
        }
    }

    public async Task<bool> UpdateAsync(int id, string title, string link, Guid createBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var menu = await db.MenuItems.FirstOrDefaultAsync(p => p.Id == id);
            if (menu == null)
            {
                return false;
            }

            menu.Title = title;
            menu.Link = link;
            menu.UpdatedBy = createBy;
            menu.UpdatedAt = DateTime.UtcNow;

            var result = await db.SaveChangesAsync();
            if (result > 0)
            {
                await cache.RemoveAsync(CacheKey);
            }

            return result > 0;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error update menu: {Exception}", exception);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(int id, int parentId, int level, Guid createBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var menu = await db.MenuItems.FirstOrDefaultAsync(p => p.Id == id);
            if (menu == null)
            {
                return false;
            }

            menu.ParentId = parentId;
            menu.Level = level;
            menu.UpdatedBy = createBy;
            menu.UpdatedAt = DateTime.UtcNow;

            var result = await db.SaveChangesAsync();
            if (result > 0)
            {
                await cache.RemoveAsync(CacheKey);
            }

            return result > 0;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error update menu: {Exception}", exception);
            return false;
        }
    }
}