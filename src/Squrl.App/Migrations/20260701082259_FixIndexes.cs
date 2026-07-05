using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class FixIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_unit_of_measures_date_created",
                table: "unit_of_measures",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_date_created",
                table: "suppliers",
                column: "date_created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_unit_of_measures_date_created",
                table: "unit_of_measures");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_date_created",
                table: "suppliers");
        }
    }
}
