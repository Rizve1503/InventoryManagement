using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedValidationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CustomNumeric1MaxValue",
                table: "Inventories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomNumeric1MinValue",
                table: "Inventories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomNumeric2MaxValue",
                table: "Inventories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomNumeric2MinValue",
                table: "Inventories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomNumeric3MaxValue",
                table: "Inventories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomNumeric3MinValue",
                table: "Inventories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomString1MaxLength",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomString1Regex",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomString2MaxLength",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomString2Regex",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomString3MaxLength",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomString3Regex",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomNumeric1MaxValue",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomNumeric1MinValue",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomNumeric2MaxValue",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomNumeric2MinValue",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomNumeric3MaxValue",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomNumeric3MinValue",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomString1MaxLength",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomString1Regex",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomString2MaxLength",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomString2Regex",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomString3MaxLength",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomString3Regex",
                table: "Inventories");
        }
    }
}
