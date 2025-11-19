namespace DirectoryService.Contracts.Common;

public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public long TotalCount { get; init; }
    public int PageSize { get; init; }
    public int Page { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
}