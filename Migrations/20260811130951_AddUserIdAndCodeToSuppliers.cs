using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdAndCodeToSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "suppliers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "suppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Code",
                table: "suppliers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_UserId",
                table: "suppliers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_users_UserId",
                table: "suppliers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_users_UserId",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_suppliers_Code",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_suppliers_UserId",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "suppliers");
        }
    }
}
