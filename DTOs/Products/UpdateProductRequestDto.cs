namespace WarehouseWeb.Api.DTOs.Products
{
    public class UpdateProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Weight { get; set; }

        public List<Guid>? CategoryIds { get; set; }
    }
}
