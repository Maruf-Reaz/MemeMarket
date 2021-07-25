using Microsoft.EntityFrameworkCore.Migrations;

namespace MemeMarketeerApp.Migrations
{
    public partial class PaymentVariationAddedForAffiliate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PaymentForPost",
                table: "Affiliates",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PaymentForPostPlusStory",
                table: "Affiliates",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PaymentForStory",
                table: "Affiliates",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentForPost",
                table: "Affiliates");

            migrationBuilder.DropColumn(
                name: "PaymentForPostPlusStory",
                table: "Affiliates");

            migrationBuilder.DropColumn(
                name: "PaymentForStory",
                table: "Affiliates");
        }
    }
}
