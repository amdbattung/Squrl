using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class RenamePurchaseOrderItemToItemInPurchaseOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_detail_items_purchase_order_item_id",
                table: "purchase_order_detail");

            migrationBuilder.RenameColumn(
                name: "purchase_order_item_id",
                table: "purchase_order_detail",
                newName: "item_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_detail_purchase_order_item_id",
                table: "purchase_order_detail",
                newName: "ix_purchase_order_detail_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_detail_items_item_id",
                table: "purchase_order_detail",
                column: "item_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_detail_items_item_id",
                table: "purchase_order_detail");

            migrationBuilder.RenameColumn(
                name: "item_id",
                table: "purchase_order_detail",
                newName: "purchase_order_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_detail_item_id",
                table: "purchase_order_detail",
                newName: "ix_purchase_order_detail_purchase_order_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_detail_items_purchase_order_item_id",
                table: "purchase_order_detail",
                column: "purchase_order_item_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
