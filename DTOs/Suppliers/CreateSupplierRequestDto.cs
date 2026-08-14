namespace WarehouseWeb.Api.DTOs.Suppliers
{
    public class CreateSupplierRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
    }
}
