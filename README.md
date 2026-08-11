# WarehouseWeb API

REST API untuk Warehouse Management System (WMS) yang dibangun dengan ASP.NET Core 8, EF Core, SQL Server, JWT authentication, dan RBAC.

## Prerequisites

- .NET SDK 8.0
- SQL Server (via Docker direkomendasikan)

## Quick Start

### 1. Jalankan SQL Server via Docker

```bash
docker run \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  --name wms-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Konfigurasi Database

```bash
dotnet ef database update
```

### 3. Jalankan API

```bash
dotnet run
```

API akan tersedia di `http://localhost:5185` (atau lihat output terminal).

## Default Users

DatabaseSeeder akan membuat 3 user default dengan password `Admin123!`:

| Email | Role |
|---|---|
| admin@example.com | admin |
| supervisor@example.com | supervisor |
| operator@example.com | warehouse_operator |

## API Endpoints

### Auth

- `POST /api/v1/auth/register` - Register user baru
- `POST /api/v1/auth/login` - Login dan dapatkan JWT token
- `GET /api/v1/auth/me` - Dapatkan info user yang login

### Users (admin only)

- `GET /api/v1/users` - List users dengan pagination
- `GET /api/v1/users/{id}` - Dapatkan user by ID
- `PATCH /api/v1/users/{id}/activate` - Aktifkan user
- `PATCH /api/v1/users/{id}/deactivate` - Non-aktifkan user
- `PATCH /api/v1/users/{id}/role` - Ganti role user

## Project Structure

```text
WarehouseWeb.Api/
  Common/          - API response, pagination, error models
  Controllers/     - HTTP controllers
  DTOs/            - Request/response DTOs
  Helpers/         - Utility classes (PasswordHasher, RoleHelper)
  Middleware/      - Request logging, global exception handler
  Models/          - Entity models
  Repositories/    - Data access layer
  Services/        - Business logic layer
  Validators/      - FluentValidation validators
  Data/            - DbContext, migrations, seeder
  Migrations/      - EF Core migrations
```

## Roles & Access

| Role | Access |
|---|---|
| `admin` | Full access: semua endpoint termasuk user management |
| `supervisor` | Lihat laporan, approve/cancel movement, tidak bisa ubah master data |
| `warehouse_operator` | Buat movement, lihat stok, lihat movement milik sendiri |

## Development

Build:

```bash
dotnet build
```

Add migration baru:

```bash
dotnet ef migrations add NamaMigration
dotnet ef database update
```
