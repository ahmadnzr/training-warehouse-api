namespace WarehouseWeb.Api.Common;

public class PaginationMeta
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public int TotalPage { get; set; }
}

public class PaginatedResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
    public PaginationMeta Meta { get; set; } = new PaginationMeta();
}
