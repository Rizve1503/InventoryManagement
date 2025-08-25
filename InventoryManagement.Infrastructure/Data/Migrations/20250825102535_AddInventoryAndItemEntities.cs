using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAndItemEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CustomString1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomString2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomString3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomText1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomText1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomText2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomText2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomText3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomText3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomNumeric1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomNumeric1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomNumeric2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomNumeric2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomNumeric3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomNumeric3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomBool1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomBool1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomBool2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomBool2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomBool3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomBool3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomLink1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomLink1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomLink2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomLink2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomLink3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomLink3Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CustomString1Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString2Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString3Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomText1Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomText2Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomText3Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomNumeric1Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CustomNumeric2Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CustomNumeric3Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CustomBool1Value = table.Column<bool>(type: "bit", nullable: true),
                    CustomBool2Value = table.Column<bool>(type: "bit", nullable: true),
                    CustomBool3Value = table.Column<bool>(type: "bit", nullable: true),
                    CustomLink1Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomLink2Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomLink3Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CreatorId",
                table: "Inventories",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_InventoryId_CustomId",
                table: "Items",
                columns: new[] { "InventoryId", "CustomId" },
                unique: true,
                filter: "[CustomId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Inventories");
        }
    }
}
