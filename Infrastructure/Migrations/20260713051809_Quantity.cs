using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Quantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "MoveOrderProducts",
                newName: "TotalQuantity");

            migrationBuilder.AddColumn<bool>(
                name: "IsTransacted",
                table: "MoveOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "MoveOrderProductWarehouseReceivings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTransacted",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.RenameColumn(
                name: "TotalQuantity",
                table: "MoveOrderProducts",
                newName: "Quantity");
        }
    }
}
