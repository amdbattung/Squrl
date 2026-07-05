using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class FixIndexForDateCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_name_description",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_name_description",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "ix_items_name_description",
                table: "items");

            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_name",
                table: "unit_of_measures",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_name",
                table: "suppliers",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_items_date_created",
                table: "items",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "ix_items_name",
                table: "items",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_name",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_name",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "ix_items_date_created",
                table: "items");

            migrationBuilder.DropIndex(
                name: "ix_items_name",
                table: "items");

            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_name_description",
                table: "unit_of_measures",
                columns: new[] { "name", "description" });

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_name_description",
                table: "suppliers",
                columns: new[] { "name", "description" });

            migrationBuilder.CreateIndex(
                name: "ix_items_name_description",
                table: "items",
                columns: new[] { "name", "description" });
        }
    }
}
