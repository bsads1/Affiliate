using Microsoft.EntityFrameworkCore;

namespace Affiliate.Application.Extensions.Paging;

public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalItemCount { get; private set; }
    public int PageRange { get; set; } = 3;
    
    public int CalculateDescendingIndex()
    {
        int totalItemCount = TotalPages * PageSize;
        if (TotalItemCount < totalItemCount)
        {
            return TotalItemCount;
        }
        else
        {
            var remainingItems = TotalItemCount % PageSize;
            return TotalItemCount - remainingItems;
        }
    }
    
    public PaginatedList() { }

    private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalItemCount = count;
        PageSize = pageSize;
        AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
    public static async Task<PaginatedList<T>> CreateAsync(List<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count;
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}