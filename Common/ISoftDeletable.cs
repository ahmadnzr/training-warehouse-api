namespace WarehouseWeb.Api.Common;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
}
