using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class TransformProductCategoryToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_product_categories_ProductCategoryId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_ProductCategoryId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_categories",
                table: "product_categories");

            migrationBuilder.DropIndex(
                name: "IX_product_categories_DeletedAt",
                table: "product_categories");

            migrationBuilder.DropIndex(
                name: "IX_product_categories_Name",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "product_categories");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "product_categories",
                newName: "CategoryId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "product_categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_categories",
                table: "product_categories",
                columns: new[] { "ProductId", "CategoryId" });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_CategoryId",
                table: "product_categories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_ProductId",
                table: "product_categories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_DeletedAt",
                table: "categories",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_categories_Name",
                table: "categories",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_product_categories_categories_CategoryId",
                table: "product_categories",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_categories_products_ProductId",
                table: "product_categories",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_categories_categories_CategoryId",
                table: "product_categories");

            migrationBuilder.DropForeignKey(
                name: "FK_product_categories_products_ProductId",
                table: "product_categories");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_categories",
                table: "product_categories");

            migrationBuilder.DropIndex(
                name: "IX_product_categories_CategoryId",
                table: "product_categories");

            migrationBuilder.DropIndex(
                name: "IX_product_categories_ProductId",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "product_categories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "product_categories",
                newName: "Id");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductCategoryId",
                table: "products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "product_categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "product_categories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "product_categories",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "product_categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_categories",
                table: "product_categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_products_ProductCategoryId",
                table: "products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_DeletedAt",
                table: "product_categories",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_Name",
                table: "product_categories",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_categories_ProductCategoryId",
                table: "products",
                column: "ProductCategoryId",
                principalTable: "product_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
