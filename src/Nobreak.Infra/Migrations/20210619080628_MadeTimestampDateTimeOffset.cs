using Microsoft.EntityFrameworkCore.Migrations;

namespace Nobreak.Migrations
{
    public partial class MadeTimestampDateTimeOffset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobreakStateChanges_NobreakStates_NobreakStateId",
                table: "NobreakStateChanges");

            migrationBuilder.DropIndex(
                name: "IX_NobreakStates_Timestamp",
                table: "NobreakStates");

            migrationBuilder.AlterColumn<byte>(
                name: "LoadPercentage",
                table: "NobreakStates",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<bool>(
                name: "OnPurpose",
                table: "NobreakStateChanges",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit");

            migrationBuilder.AlterColumn<long>(
                name: "NobreakStateId",
                table: "NobreakStateChanges",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_NobreakStates_Timestamp",
                table: "NobreakStates",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_NobreakStateChanges_Timestamp",
                table: "NobreakStateChanges",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Timestamp",
                table: "Accounts",
                column: "Timestamp");

            migrationBuilder.AddForeignKey(
                name: "FK_NobreakStateChanges_NobreakStates_NobreakStateId",
                table: "NobreakStateChanges",
                column: "NobreakStateId",
                principalTable: "NobreakStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobreakStateChanges_NobreakStates_NobreakStateId",
                table: "NobreakStateChanges");

            migrationBuilder.DropIndex(
                name: "IX_NobreakStates_Timestamp",
                table: "NobreakStates");

            migrationBuilder.DropIndex(
                name: "IX_NobreakStateChanges_Timestamp",
                table: "NobreakStateChanges");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Timestamp",
                table: "Accounts");

            migrationBuilder.AlterColumn<byte>(
                name: "LoadPercentage",
                table: "NobreakStates",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<ulong>(
                name: "OnPurpose",
                table: "NobreakStateChanges",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<long>(
                name: "NobreakStateId",
                table: "NobreakStateChanges",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NobreakStates_Timestamp",
                table: "NobreakStates",
                column: "Timestamp",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NobreakStateChanges_NobreakStates_NobreakStateId",
                table: "NobreakStateChanges",
                column: "NobreakStateId",
                principalTable: "NobreakStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
