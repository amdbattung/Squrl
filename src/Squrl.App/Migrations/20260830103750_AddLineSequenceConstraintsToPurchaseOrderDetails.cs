using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddLineSequenceConstraintsToPurchaseOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_purchase_order_details_purchase_order_id",
                table: "purchase_order_details");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_details_purchase_order_id_line_sequence",
                table: "purchase_order_details",
                columns: new[] { "purchase_order_id", "line_sequence" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_purchase_order_details_line_sequence",
                table: "purchase_order_details",
                sql: "line_sequence >= 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_purchase_order_details_purchase_order_id_line_sequence",
                table: "purchase_order_details");

            migrationBuilder.DropCheckConstraint(
                name: "CK_purchase_order_details_line_sequence",
                table: "purchase_order_details");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_details_purchase_order_id",
                table: "purchase_order_details",
                column: "purchase_order_id");
        }
    }
}
