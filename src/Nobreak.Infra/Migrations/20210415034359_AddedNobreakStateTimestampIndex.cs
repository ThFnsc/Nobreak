using Microsoft.EntityFrameworkCore.Migrations;

namespace Nobreak.Migrations
{
    public partial class AddedNobreakStateTimestampIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateIndex(
                name: "IX_NobreakStates_Timestamp",
                table: "NobreakStates",
                column: "Timestamp",
                unique: true);

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropIndex(
                name: "IX_NobreakStates_Timestamp",
                table: "NobreakStates");
    }
}
