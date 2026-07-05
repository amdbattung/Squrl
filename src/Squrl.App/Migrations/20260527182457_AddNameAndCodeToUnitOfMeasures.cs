using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndCodeToUnitOfMeasures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "unit",
                table: "unit_of_measures",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "ix_unit_of_measures_unit_description",
                table: "unit_of_measures",
                newName: "ix_unit_of_measures_name_description");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "unit_of_measures",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "unit_of_measures");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "unit_of_measures",
                newName: "unit");

            migrationBuilder.RenameIndex(
                name: "ix_unit_of_measures_name_description",
                table: "unit_of_measures",
                newName: "ix_unit_of_measures_unit_description");
        }
    }
}
