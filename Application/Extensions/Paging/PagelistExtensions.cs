namespace Affiliate.Application.Extensions.Paging;

public static class PagelistExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize)
    {
        return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
    }
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this List<T> query, int pageIndex, int pageSize)
    {
        return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
    }
}