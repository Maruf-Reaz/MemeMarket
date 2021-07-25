using Microsoft.EntityFrameworkCore.Migrations;

namespace MemeMarketeerApp.Migrations
{
    public partial class COmmentsChar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReactionGenerated",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "CommentsGenerated",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ReactionGenerated",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CommentsGenerated",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
