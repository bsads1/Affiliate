using System.Text.Json;
using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Enums;
using Affiliate.Application.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Affiliate.Application.Services;

public class PageService(IDbContextFactory<DatabaseContext> factory)
{
    public async Task<PageItem> DeletePageAsync(int id, Guid updateBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var user = await db.PageItems.FindAsync(id);
            if (user != null)
            {
                user.IsDelete = true;
                user.UpdatedAt = DateTime.Now.ToUniversalTime();
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

    public async Task<PageItem?> CreatePageAsync(PagePostFormDto form)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var page = form.Adapt<PageItem>();
            page.Type = PageItemType.Page.ToString();
            page.Tags = JsonSerializer.Serialize(form.Tags);
            await db.PageItems.AddAsync(page);
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

    public async Task<PageItem?> UpdatePageAsync(PagePostFormDto form)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var page = await db.PageItems.FindAsync(form.Id);
            page.UpdatedAt = DateTime.Now.ToUniversalTime();
            page.UpdatedBy = form.UpdatedBy;
            page.Name = form.Name;
            page.Slug = form.Slug;
            page.Content = form.Content;
            page.Description = form.Description;
            page.Tags = JsonSerializer.Serialize(form.Tags);
            if (form.HasImage)
            {
                page.Image = form.Image;
            }

            if (form.HasBanner)
            {
                page.Banner = form.Banner;
            }

            db.PageItems.Update(page);
            await db.SaveChangesAsync();
            return page;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return null;
        }
    }

    public async Task<PageItem?> UpdateBlogAsync(PagePostFormDto form)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var page = await db.PageItems.FindAsync(form.Id);
            page.UpdatedAt = DateTime.Now.ToUniversalTime();
            page.UpdatedBy = form.UpdatedBy;
            page.Name = form.Name;
            page.Slug = form.Slug;
            page.Content = form.Content;
            page.Description = form.Description;
            page.Tags = JsonSerializer.Serialize(form.Tags);
            if (form.HasImage)
            {
                page.Image = form.Image;
            }

            if (form.HasBanner)
            {
                page.Banner = form.Banner;
            }

            db.PageItems.Update(page);
            await db.SaveChangesAsync();
            return page;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config blog: {Exception}", exception);
            return null;
        }
    }

    public async Task<PageItem?> CreateBlogAsync(PagePostFormDto form)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            var page = form.Adapt<PageItem>();
            page.Type = PageItemType.Blog.ToString();
            page.Tags = JsonSerializer.Serialize(form.Tags);
            await db.PageItems.AddAsync(page);
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
}