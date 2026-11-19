using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToSalesOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sales_order_details_sales_order_id_item_id",
                table: "sales_order_details",
                columns: new[] { "sales_order_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_details_purchase_order_id_item_id",
                table: "purchase_order_details",
                columns: new[] { "purchase_order_id", "item_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sales_order_details_sales_order_id_item_id",
                table: "sales_order_details");

            migrationBuilder.DropIndex(
                name: "ix_purchase_order_details_purchase_order_id_item_id",
                table: "purchase_order_details");
        }
    }
}
