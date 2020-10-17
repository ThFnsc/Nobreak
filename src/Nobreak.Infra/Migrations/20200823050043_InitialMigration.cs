using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Nobreak.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NobreakStates",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    VoltageIn = table.Column<float>(nullable: false),
                    VoltageOut = table.Column<float>(nullable: false),
                    LoadPercentage = table.Column<byte>(nullable: false),
                    FrequencyHz = table.Column<float>(nullable: false),
                    BatteryVoltage = table.Column<float>(nullable: false),
                    TemperatureC = table.Column<float>(nullable: false),
                    Extras = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NobreakStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NobreakStateChanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    OnPurpose = table.Column<bool>(nullable: false),
                    NobreakStateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NobreakStateChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NobreakStateChanges_NobreakStates_NobreakStateId",
                        column: x => x.NobreakStateId,
                        principalTable: "NobreakStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NobreakStateChanges_NobreakStateId",
                table: "NobreakStateChanges",
                column: "NobreakStateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "NobreakStateChanges");

            migrationBuilder.DropTable(
                name: "NobreakStates");
        }
    }
}
