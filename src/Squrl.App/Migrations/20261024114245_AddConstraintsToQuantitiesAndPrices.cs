using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsToQuantitiesAndPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_code",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_name",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_name",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "ix_items_name",
                table: "items");

            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_code",
                table: "unit_of_measures",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_name",
                table: "unit_of_measures",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_name",
                table: "suppliers",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_invoice_number",
                table: "sales_orders",
                column: "invoice_number");

            migrationBuilder.AddCheckConstraint(
                name: "CK_sales_order_details_quantity",
                table: "sales_order_details",
                sql: "quantity >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_sales_order_details_unit_price",
                table: "sales_order_details",
                sql: "unit_price >= 0");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_invoice_number",
                table: "purchase_orders",
                column: "invoice_number");

            migrationBuilder.AddCheckConstraint(
                name: "CK_purchase_order_details_quantity",
                table: "purchase_order_details",
                sql: "quantity >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_purchase_order_details_unit_price",
                table: "purchase_order_details",
                sql: "unit_price >= 0");

            migrationBuilder.CreateIndex(
                name: "ix_items_name",
                table: "items",
                column: "name",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_cost_price",
                table: "items",
                sql: "cost_price >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_list_price",
                table: "items",
                sql: "list_price >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_retail_price",
                table: "items",
                sql: "retail_price >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_code",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_name",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_name",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "ix_sales_orders_invoice_number",
                table: "sales_orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_sales_order_details_quantity",
                table: "sales_order_details");

            migrationBuilder.DropCheckConstraint(
                name: "CK_sales_order_details_unit_price",
                table: "sales_order_details");

            migrationBuilder.DropIndex(
                name: "ix_purchase_orders_invoice_number",
                table: "purchase_orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_purchase_order_details_quantity",
                table: "purchase_order_details");

            migrationBuilder.DropCheckConstraint(
                name: "CK_purchase_order_details_unit_price",
                table: "purchase_order_details");

            migrationBuilder.DropIndex(
                name: "ix_items_name",
                table: "items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_items_cost_price",
                table: "items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_items_list_price",
                table: "items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_items_retail_price",
                table: "items");

            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_code",
                table: "unit_of_measures",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_name",
                table: "unit_of_measures",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_name",
                table: "suppliers",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_items_name",
                table: "items",
                column: "name");
        }
    }
}
