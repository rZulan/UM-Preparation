using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MiscellaneousReceiptReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "MiscellaneousReceipt",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "MiscellaneousReceipt",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MiscellaneousReceipt_WarehouseId",
                table: "MiscellaneousReceipt",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropIndex(
                name: "IX_MiscellaneousReceipt_WarehouseId",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "MiscellaneousReceipt");
        }
    }
}
