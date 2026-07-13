using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMoveOrderProductWarehouseReceivingsCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, drop the foreign key and index from MoveOrderProductWarehouseReceivings
            migrationBuilder.DropForeignKey(
                name: "FK_MoveOrderProductWarehouseReceivings_MoveOrderProducts_MoveOrderProductId",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoveOrderProductWarehouseReceivings",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_MoveOrderProductId",
                table: "MoveOrderProductWarehouseReceivings");

            // Now handle MoveOrderProducts table transformation to composite key
            migrationBuilder.DropPrimaryKey(
                name: "PK_MoveOrderProducts",
                table: "MoveOrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_MoveOrderProducts_ProductId",
                table: "MoveOrderProducts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MoveOrderProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoveOrderProducts",
                table: "MoveOrderProducts",
                columns: new[] { "MoveOrderId", "ProductId" });

            // Update MoveOrderProductWarehouseReceivings
            migrationBuilder.RenameColumn(
                name: "MoveOrderProductId",
                table: "MoveOrderProductWarehouseReceivings",
                newName: "ProductId");

            migrationBuilder.AddColumn<int>(
                name: "MoveOrderId",
                table: "MoveOrderProductWarehouseReceivings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoveOrderProductWarehouseReceivings",
                table: "MoveOrderProductWarehouseReceivings",
                columns: new[] { "MoveOrderId", "ProductId", "WarehouseReceivingId" });

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProducts_ProductId",
                table: "MoveOrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_ProductId",
                table: "MoveOrderProductWarehouseReceivings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_WarehouseReceivingId",
                table: "MoveOrderProductWarehouseReceivings",
                column: "WarehouseReceivingId");

            migrationBuilder.AddForeignKey(
                name: "FK_MoveOrderProducts_MoveOrders_MoveOrderId",
                table: "MoveOrderProducts",
                column: "MoveOrderId",
                principalTable: "MoveOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoveOrderProducts_Products_ProductId",
                table: "MoveOrderProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoveOrderProductWarehouseReceivings_MoveOrderProducts_MoveOrderId_ProductId",
                table: "MoveOrderProductWarehouseReceivings",
                columns: new[] { "MoveOrderId", "ProductId" },
                principalTable: "MoveOrderProducts",
                principalColumns: new[] { "MoveOrderId", "ProductId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoveOrderProductWarehouseReceivings_WarehouseReceivings_WarehouseReceivingId",
                table: "MoveOrderProductWarehouseReceivings",
                column: "WarehouseReceivingId",
                principalTable: "WarehouseReceivings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoveOrderProducts_MoveOrders_MoveOrderId",
                table: "MoveOrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_MoveOrderProducts_Products_ProductId",
                table: "MoveOrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_MoveOrderProductWarehouseReceivings_MoveOrderProducts_MoveOrderId_ProductId",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropForeignKey(
                name: "FK_MoveOrderProductWarehouseReceivings_WarehouseReceivings_WarehouseReceivingId",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoveOrderProducts",
                table: "MoveOrderProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoveOrderProductWarehouseReceivings",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropIndex(
                name: "IX_MoveOrderProducts_ProductId",
                table: "MoveOrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_ProductId",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_WarehouseReceivingId",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MoveOrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoveOrderProducts",
                table: "MoveOrderProducts",
                column: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "MoveOrderProductWarehouseReceivings",
                newName: "MoveOrderProductId");

            migrationBuilder.DropColumn(
                name: "MoveOrderId",
                table: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoveOrderProductWarehouseReceivings",
                table: "MoveOrderProductWarehouseReceivings",
                columns: new[] { "MoveOrderProductId", "WarehouseReceivingId" });

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProducts_ProductId",
                table: "MoveOrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_MoveOrderProductId",
                table: "MoveOrderProductWarehouseReceivings",
                column: "MoveOrderProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_MoveOrderProductWarehouseReceivings_MoveOrderProducts_MoveOrderProductId",
                table: "MoveOrderProductWarehouseReceivings",
                column: "MoveOrderProductId",
                principalTable: "MoveOrderProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
