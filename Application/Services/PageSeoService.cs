using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Application.Services;

public class PageSeoService(IDbContextFactory<DatabaseContext> factory)
{
    public async Task<PageSeoPostFormDto> CreateOrUpdateSeoAsync(PageSeoPostFormDto form)
    {
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            if (form.Id > 0)
            {
                var old = await db.PageSeos.Where(p => p.Id == form.Id)
                    .FirstOrDefaultAsync();
                if (old != null)
                {
                    await UpdatePageSeo(form, old);
                    db.PageSeos.Update(old);
                    await db.SaveChangesAsync();
                    return old.Adapt<PageSeoPostFormDto>();
                }
            }
            else
            {
                var old = await db.PageSeos.Where(p => p.PageRoute == form.PageRoute)
                    .FirstOrDefaultAsync();
                if (old != null)
                {
                    await UpdatePageSeo(form, old);
                    db.PageSeos.Update(old);
                    await db.SaveChangesAsync();
                    return old.Adapt<PageSeoPostFormDto>();
                }
                else
                {
                    var pageSeo = await CreatePageSeo(form);
                    await db.PageSeos.AddAsync(pageSeo);
                    await db.SaveChangesAsync();
                    return pageSeo.Adapt<PageSeoPostFormDto>();
                }
            }

            return new PageSeoPostFormDto() { Error = "Page not found" };
        }
        catch (Exception e)
        {
            return new PageSeoPostFormDto() { Error = e.Message };
        }
    }

    private async Task<PageSeo> CreatePageSeo(PageSeoPostFormDto form)
    {
        var pageSeo = new PageSeo()
        {
            Guid = GuidExtension.TaoGuid(),
            PageId = form.PageId,
            PageType = form.PageType,
            PageRoute = form.PageRoute,
            Title = form.Title,
            Keywords = form.Keywords,
            Description = form.Description,
            Canonical = form.Canonical,
            Robots = form.Robots,
            Image = form.HasImage ? form.Image : "",
            CreatedBy = form.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = form.UpdatedBy,
            UpdatedAt = DateTime.UtcNow
        };
        return pageSeo;
    }

    private async Task<PageSeo> UpdatePageSeo(PageSeoPostFormDto form, PageSeo old)
    {
        old.UpdatedAt = DateTime.UtcNow;
        old.UpdatedBy = form.UpdatedBy;
        old.PageRoute = form.PageRoute;
        old.Title = form.Title;
        old.Keywords = form.Keywords;
        old.Description = form.Description;
        old.Canonical = form.Canonical;
        old.Robots = form.Robots;
        old.Image = form.HasImage ? form.Image : "";
        return old;
    }
}