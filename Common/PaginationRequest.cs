namespace WarehouseWeb.Api.Common;

public class PaginationRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public string Sort { get; set; } = "updated_at";
    public string Order { get; set; } = "DESC";
    public string? Search { get; set; }

    public int GetOffset() => (Page - 1) * PerPage;

    public void Validate()
    {
        Page = Math.Max(1, Page);
        PerPage = Math.Clamp(PerPage, 1, 100);
    }
}
