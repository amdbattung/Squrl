using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squrl.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityAndLocationsToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "date_created",
                table: "unit_of_measures",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "date_created",
                table: "items",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AddColumn<string>(
                name: "locations",
                table: "items",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "locations",
                table: "items");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "items");

            migrationBuilder.AlterColumn<string>(
                name: "date_created",
                table: "unit_of_measures",
                type: "TEXT",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "date_created",
                table: "items",
                type: "TEXT",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
