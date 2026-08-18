using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyStockReportAndJobLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_execution_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_execution_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "daily_stock_reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JobExecutionLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_stock_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_stock_reports_job_execution_logs_JobExecutionLogId",
                        column: x => x.JobExecutionLogId,
                        principalTable: "job_execution_logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "daily_stock_report_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyStockReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_stock_report_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_stock_report_items_daily_stock_reports_DailyStockReportId",
                        column: x => x.DailyStockReportId,
                        principalTable: "daily_stock_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_daily_stock_report_items_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_report_items_DailyStockReportId_ProductId",
                table: "daily_stock_report_items",
                columns: new[] { "DailyStockReportId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_report_items_ProductId",
                table: "daily_stock_report_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_reports_JobExecutionLogId",
                table: "daily_stock_reports",
                column: "JobExecutionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_reports_ReportDate",
                table: "daily_stock_reports",
                column: "ReportDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_execution_logs_JobName",
                table: "job_execution_logs",
                column: "JobName");

            migrationBuilder.CreateIndex(
                name: "IX_job_execution_logs_StartedAt",
                table: "job_execution_logs",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_job_execution_logs_Status",
                table: "job_execution_logs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_stock_report_items");

            migrationBuilder.DropTable(
                name: "daily_stock_reports");

            migrationBuilder.DropTable(
                name: "job_execution_logs");
        }
    }
}
