using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class FixPurchaseOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_suppliers_supplier_id",
                table: "purchase_order");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_detail_items_item_id",
                table: "purchase_order_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_detail_purchase_order_purchase_order_id",
                table: "purchase_order_detail");

            migrationBuilder.DropPrimaryKey(
                name: "pk_purchase_order_detail",
                table: "purchase_order_detail");

            migrationBuilder.DropPrimaryKey(
                name: "pk_purchase_order",
                table: "purchase_order");

            migrationBuilder.RenameTable(
                name: "purchase_order_detail",
                newName: "purchase_order_details");

            migrationBuilder.RenameTable(
                name: "purchase_order",
                newName: "purchase_orders");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_detail_purchase_order_id",
                table: "purchase_order_details",
                newName: "ix_purchase_order_details_purchase_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_detail_item_id",
                table: "purchase_order_details",
                newName: "ix_purchase_order_details_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_detail_date_created",
                table: "purchase_order_details",
                newName: "ix_purchase_order_details_date_created");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_supplier_id",
                table: "purchase_orders",
                newName: "ix_purchase_orders_supplier_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_date_created",
                table: "purchase_orders",
                newName: "ix_purchase_orders_date_created");

            migrationBuilder.AddPrimaryKey(
                name: "pk_purchase_order_details",
                table: "purchase_order_details",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_purchase_orders",
                table: "purchase_orders",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_details_items_item_id",
                table: "purchase_order_details",
                column: "item_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_details_purchase_orders_purchase_order_id",
                table: "purchase_order_details",
                column: "purchase_order_id",
                principalTable: "purchase_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_orders_suppliers_supplier_id",
                table: "purchase_orders",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_details_items_item_id",
                table: "purchase_order_details");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_details_purchase_orders_purchase_order_id",
                table: "purchase_order_details");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_orders_suppliers_supplier_id",
                table: "purchase_orders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_purchase_orders",
                table: "purchase_orders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_purchase_order_details",
                table: "purchase_order_details");

            migrationBuilder.RenameTable(
                name: "purchase_orders",
                newName: "purchase_order");

            migrationBuilder.RenameTable(
                name: "purchase_order_details",
                newName: "purchase_order_detail");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_orders_supplier_id",
                table: "purchase_order",
                newName: "ix_purchase_order_supplier_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_orders_date_created",
                table: "purchase_order",
                newName: "ix_purchase_order_date_created");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_details_purchase_order_id",
                table: "purchase_order_detail",
                newName: "ix_purchase_order_detail_purchase_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_details_item_id",
                table: "purchase_order_detail",
                newName: "ix_purchase_order_detail_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_details_date_created",
                table: "purchase_order_detail",
                newName: "ix_purchase_order_detail_date_created");

            migrationBuilder.AddPrimaryKey(
                name: "pk_purchase_order",
                table: "purchase_order",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_purchase_order_detail",
                table: "purchase_order_detail",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_suppliers_supplier_id",
                table: "purchase_order",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_detail_items_item_id",
                table: "purchase_order_detail",
                column: "item_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_detail_purchase_order_purchase_order_id",
                table: "purchase_order_detail",
                column: "purchase_order_id",
                principalTable: "purchase_order",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
