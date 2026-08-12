namespace WarehouseWeb.Api.DTOs.WarehouseLocations;

public class CreateWarehouseLocationRequestDto
{
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
