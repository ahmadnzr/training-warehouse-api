namespace WarehouseWeb.Api.Models;

public class Supplier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Kode unik supplier, misal SUP-001
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Foreign key ke tabel users (Nullable: tidak semua supplier wajib punya akun login)
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
