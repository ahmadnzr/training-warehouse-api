namespace WarehouseWeb.Api.DTOs.Products
{
    public class ProductDto
    {
        public Guid Id { set; get; }
        public string Sku { set; get; } = string.Empty;
        public string Name { set; get; } = string.Empty;
        public string Unit { set; get; } = string.Empty;
        public decimal Weight { set; get; }
        public bool IsActive { set; get; }

        public List<string> CategoryNames { set; get; } = new();
    }
}
