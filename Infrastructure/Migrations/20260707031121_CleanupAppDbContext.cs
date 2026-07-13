using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanupAppDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Products_ProductId",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Users_CreatedById",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Users_UpdatedById",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseReceivings_MiscellaneousReceipt_MiscellaneousReceiptId",
                table: "WarehouseReceivings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MiscellaneousReceipt",
                table: "MiscellaneousReceipt");

            migrationBuilder.RenameTable(
                name: "MiscellaneousReceipt",
                newName: "MiscellaneousReceipts");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipt_WarehouseId",
                table: "MiscellaneousReceipts",
                newName: "IX_MiscellaneousReceipts_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipt_UpdatedById",
                table: "MiscellaneousReceipts",
                newName: "IX_MiscellaneousReceipts_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipt_ProductId",
                table: "MiscellaneousReceipts",
                newName: "IX_MiscellaneousReceipts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipt_CreatedById",
                table: "MiscellaneousReceipts",
                newName: "IX_MiscellaneousReceipts_CreatedById");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "WarehouseReceivings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Uoms",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Permissions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PendingAccounts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "MoveOrders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "MiscellaneousReceipts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MiscellaneousReceipts",
                table: "MiscellaneousReceipts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipts_Products_ProductId",
                table: "MiscellaneousReceipts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipts_Users_CreatedById",
                table: "MiscellaneousReceipts",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipts_Users_UpdatedById",
                table: "MiscellaneousReceipts",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipts_Warehouses_WarehouseId",
                table: "MiscellaneousReceipts",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseReceivings_MiscellaneousReceipts_MiscellaneousReceiptId",
                table: "WarehouseReceivings",
                column: "MiscellaneousReceiptId",
                principalTable: "MiscellaneousReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipts_Products_ProductId",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipts_Users_CreatedById",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipts_Users_UpdatedById",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MiscellaneousReceipts_Warehouses_WarehouseId",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseReceivings_MiscellaneousReceipts_MiscellaneousReceiptId",
                table: "WarehouseReceivings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MiscellaneousReceipts",
                table: "MiscellaneousReceipts");

            migrationBuilder.RenameTable(
                name: "MiscellaneousReceipts",
                newName: "MiscellaneousReceipt");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipts_WarehouseId",
                table: "MiscellaneousReceipt",
                newName: "IX_MiscellaneousReceipt_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipts_UpdatedById",
                table: "MiscellaneousReceipt",
                newName: "IX_MiscellaneousReceipt_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipts_ProductId",
                table: "MiscellaneousReceipt",
                newName: "IX_MiscellaneousReceipt_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_MiscellaneousReceipts_CreatedById",
                table: "MiscellaneousReceipt",
                newName: "IX_MiscellaneousReceipt_CreatedById");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "WarehouseReceivings",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Uoms",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PendingAccounts",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "MoveOrders",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "MiscellaneousReceipt",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MiscellaneousReceipt",
                table: "MiscellaneousReceipt",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Products_ProductId",
                table: "MiscellaneousReceipt",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Users_CreatedById",
                table: "MiscellaneousReceipt",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Users_UpdatedById",
                table: "MiscellaneousReceipt",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiscellaneousReceipt_Warehouses_WarehouseId",
                table: "MiscellaneousReceipt",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseReceivings_MiscellaneousReceipt_MiscellaneousReceiptId",
                table: "WarehouseReceivings",
                column: "MiscellaneousReceiptId",
                principalTable: "MiscellaneousReceipt",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
