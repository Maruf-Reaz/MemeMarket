using Microsoft.EntityFrameworkCore.Migrations;

namespace MemeMarketeerApp.Migrations
{
    public partial class followerssChar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TotalFollwers",
                table: "Affiliates",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalFollwers",
                table: "Affiliates",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
