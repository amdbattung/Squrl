using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "invoice_number",
                table: "purchase_orders",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                table: "purchase_order_details",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "cost_price",
                table: "items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "list_price",
                table: "items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "retail_price",
                table: "items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sales_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    customer = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    invoice_number = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    date_ordered = table.Column<string>(type: "TEXT", nullable: false),
                    date_created = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sales_order_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    sales_order_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    line_sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    item_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    unit_price = table.Column<decimal>(type: "TEXT", nullable: false),
                    quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    date_created = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales_order_details", x => x.id);
                    table.CheckConstraint("CK_sales_order_details_line_sequence", "line_sequence >= 1");
                    table.ForeignKey(
                        name: "fk_sales_order_details_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sales_order_details_sales_orders_sales_order_id",
                        column: x => x.sales_order_id,
                        principalTable: "sales_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_details_date_created",
                table: "sales_order_details",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_details_item_id",
                table: "sales_order_details",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_details_sales_order_id_line_sequence",
                table: "sales_order_details",
                columns: new[] { "sales_order_id", "line_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_date_created",
                table: "sales_orders",
                column: "date_created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sales_order_details");

            migrationBuilder.DropTable(
                name: "sales_orders");

            migrationBuilder.DropColumn(
                name: "invoice_number",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "purchase_order_details");

            migrationBuilder.DropColumn(
                name: "cost_price",
                table: "items");

            migrationBuilder.DropColumn(
                name: "list_price",
                table: "items");

            migrationBuilder.DropColumn(
                name: "retail_price",
                table: "items");
        }
    }
}
