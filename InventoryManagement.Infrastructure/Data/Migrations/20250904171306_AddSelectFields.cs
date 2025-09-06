using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomSelect1Value",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect2Value",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect3Value",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect1Name",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect1Options",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "CustomSelect1State",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect2Name",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect2Options",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "CustomSelect2State",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect3Name",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomSelect3Options",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "CustomSelect3State",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomSelect1Value",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CustomSelect2Value",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CustomSelect3Value",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CustomSelect1Name",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect1Options",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect1State",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect2Name",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect2Options",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect2State",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect3Name",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect3Options",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CustomSelect3State",
                table: "Inventories");
        }
    }
}
