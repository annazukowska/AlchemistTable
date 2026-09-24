using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlchemistTable.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alchemists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alchemists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Potions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    EffectPower = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Potions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AlchemistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlchemistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Alchemists_AlchemistId",
                        column: x => x.AlchemistId,
                        principalTable: "Alchemists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ingredients_Potions_PotionId",
                        column: x => x.PotionId,
                        principalTable: "Potions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alchemists_Email",
                table: "Alchemists",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_AlchemistId",
                table: "Ingredients",
                column: "AlchemistId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_PotionId",
                table: "Ingredients",
                column: "PotionId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Alchemists");

            migrationBuilder.DropTable(
                name: "Potions");
        }
    }
}
