using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMiscellaneousReceiptWarehouseFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt");

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt");

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
