using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddDateCreatedAndIndexesToPurchaseOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "date_created",
                table: "purchase_order_detail",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "date_created",
                table: "purchase_order",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_detail_date_created",
                table: "purchase_order_detail",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_date_created",
                table: "purchase_order",
                column: "date_created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_purchase_order_detail_date_created",
                table: "purchase_order_detail");

            migrationBuilder.DropIndex(
                name: "ix_purchase_order_date_created",
                table: "purchase_order");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "purchase_order_detail");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "purchase_order");
        }
    }
}
