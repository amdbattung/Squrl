using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchase_order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    supplier_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    date_ordered = table.Column<string>(type: "TEXT", nullable: false),
                    date_required = table.Column<string>(type: "TEXT", nullable: true),
                    date_shipped = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_order", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_order_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_order_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    purchase_order_item_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    quantity = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_order_detail", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_order_detail_items_purchase_order_item_id",
                        column: x => x.purchase_order_item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_purchase_order_detail_purchase_order_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_supplier_id",
                table: "purchase_order",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_detail_purchase_order_id",
                table: "purchase_order_detail",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_detail_purchase_order_item_id",
                table: "purchase_order_detail",
                column: "purchase_order_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchase_order_detail");

            migrationBuilder.DropTable(
                name: "purchase_order");
        }
    }
}
