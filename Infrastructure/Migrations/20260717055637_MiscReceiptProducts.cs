using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MiscReceiptProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipts_Products_ProductId",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropIndex(
                name: "IX_MiscellaneousReceipts_ProductId",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "MiscellaneousReceipts");

            migrationBuilder.CreateTable(
                name: "MiscellaneousReceiptProducts",
                columns: table => new
                {
                    MiscellaneousReceiptId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiscellaneousReceiptProducts", x => new { x.MiscellaneousReceiptId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_MiscellaneousReceiptProducts_MiscellaneousReceipts_MiscellaneousReceiptId",
                        column: x => x.MiscellaneousReceiptId,
                        principalTable: "MiscellaneousReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiscellaneousReceiptProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MiscellaneousReceiptProducts_ProductId",
                table: "MiscellaneousReceiptProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiscellaneousReceiptProducts");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "MiscellaneousReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "MiscellaneousReceipts",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_MiscellaneousReceipts_ProductId",
                table: "MiscellaneousReceipts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipts_Products_ProductId",
                table: "MiscellaneousReceipts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
