using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Helpers;

public static class RoleHelper
{
    public static string? ToSnakeCaseRole(UserRole? role)
    {
        return role switch
        {
            UserRole.Admin => "admin",
            UserRole.Supervisor => "supervisor",
            UserRole.WarehouseOperator => "warehouse_operator",
            _ => null
        };
    }

    public static UserRole? ParseSnakeCaseRole(string? role)
    {
        return role?.ToLowerInvariant() switch
        {
            "admin" => UserRole.Admin,
            "supervisor" => UserRole.Supervisor,
            "warehouse_operator" => UserRole.WarehouseOperator,
            _ => null
        };
    }

    public static IReadOnlyList<string> ValidRoles => new[] { "admin", "supervisor", "warehouse_operator" };
}
