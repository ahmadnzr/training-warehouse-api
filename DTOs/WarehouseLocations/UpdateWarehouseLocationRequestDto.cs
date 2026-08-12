namespace WarehouseWeb.Api.DTOs.WarehouseLocations;

public class UpdateWarehouseLocationRequestDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
