using Microsoft.EntityFrameworkCore.Migrations;

namespace Nobreak.Migrations
{
    public partial class NonNullablePassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Accounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
    }
}
