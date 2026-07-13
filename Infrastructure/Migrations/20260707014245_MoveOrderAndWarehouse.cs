using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveOrderAndWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoveOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoveOrders_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoveOrders_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoveOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MoveOrderProducts",
                columns: table => new
                {
                    MoveOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveOrderProducts", x => new { x.MoveOrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_MoveOrderProducts_MoveOrders_MoveOrderId",
                        column: x => x.MoveOrderId,
                        principalTable: "MoveOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoveOrderProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MoveOrderProductWarehouseReceivings",
                columns: table => new
                {
                    MoveOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseReceivingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveOrderProductWarehouseReceivings", x => new { x.MoveOrderId, x.ProductId, x.WarehouseReceivingId });
                    table.ForeignKey(
                        name: "FK_MoveOrderProductWarehouseReceivings_MoveOrderProducts_MoveOrderId_ProductId",
                        columns: x => new { x.MoveOrderId, x.ProductId },
                        principalTable: "MoveOrderProducts",
                        principalColumns: new[] { "MoveOrderId", "ProductId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoveOrderProductWarehouseReceivings_WarehouseReceivings_WarehouseReceivingId",
                        column: x => x.WarehouseReceivingId,
                        principalTable: "WarehouseReceivings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProducts_ProductId",
                table: "MoveOrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrderProductWarehouseReceivings_WarehouseReceivingId",
                table: "MoveOrderProductWarehouseReceivings",
                column: "WarehouseReceivingId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrders_CreatedById",
                table: "MoveOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrders_UpdatedById",
                table: "MoveOrders",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOrders_WarehouseId",
                table: "MoveOrders",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoveOrderProductWarehouseReceivings");

            migrationBuilder.DropTable(
                name: "MoveOrderProducts");

            migrationBuilder.DropTable(
                name: "MoveOrders");
        }
    }
}
