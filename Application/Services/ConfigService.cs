using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZiggyCreatures.Caching.Fusion;

namespace Affiliate.Application.Services;

public class ConfigService(IDbContextFactory<DatabaseContext> factory, IFusionCache cache)
{
    public async Task<ConfigPostFormDto?> GetConfig()
    {
        var config = await cache.GetOrSetAsync<ConfigPostFormDto>("Cache:Config", async ctx =>
        {
            await using var db = await factory.CreateDbContextAsync(ctx);
            var config = await db.ConfigPages.FirstOrDefaultAsync(ctx);
            if (config == null)
            {
                return new ConfigPostFormDto();
            }

            return config.Adapt<ConfigPostFormDto>();
        }, TimeSpan.FromMinutes(10));
        return config;
    }

    public async Task<ConfigPostFormDto?> UpdateConfig(ConfigPostFormDto configPostFormDto)
    {
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            var config = await db.ConfigPages.FirstOrDefaultAsync();
            if (config == null)
            {
                config = configPostFormDto.Adapt<ConfigPage>();
                config.CreatedBy = configPostFormDto.CreatedBy;
                config.CreatedAt = DateTime.Now.ToUniversalTime();
                await db.ConfigPages.AddAsync(config);
            }
            else
            {
                config.PageName = configPostFormDto.PageName ?? "";
                config.Description = configPostFormDto.Description ?? "";
                config.Author = configPostFormDto.Author ?? "";
                config.Keywords = configPostFormDto.Keywords ?? "";
                config.AdditionalMeta = configPostFormDto.AdditionalMeta ?? "";
                config.CustomCss = configPostFormDto.CustomCss ?? "";
                config.CustomJs = configPostFormDto.CustomJs ?? "";
                if (configPostFormDto.LogoHasData)
                {
                    config.Logo = configPostFormDto.Logo ?? "";
                }

                if (configPostFormDto.FaviconHasData)
                {
                    config.Favicon = configPostFormDto.Favicon ?? "";
                }

                config.UpdatedBy = configPostFormDto.CreatedBy;
                config.UpdatedAt = DateTime.Now.ToUniversalTime();
            }

            await db.SaveChangesAsync();
            await cache.RemoveAsync("Cache:Config");
            return config.Adapt<ConfigPostFormDto>();
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return null;
        }
    }
}