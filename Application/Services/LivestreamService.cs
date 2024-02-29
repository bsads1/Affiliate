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
            var user = await db.Livestreams.FindAsync(id);
            if (user != null)
            {
                user.IsDelete = true;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updateBy;
                await db.SaveChangesAsync();
                return user;
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
            var page = form.Adapt<Livestream>();
            await db.Livestreams.AddAsync(page);
            await db.SaveChangesAsync();
            return page;
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
                livestream.IsShow = form.IsShow;
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
}