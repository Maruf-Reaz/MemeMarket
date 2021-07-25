using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MemeMarketeerApp.Migrations
{
    public partial class CatagoryAndLanguageAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BitlyLink",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AffiliateLanguageId",
                table: "Affiliates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatagoryId",
                table: "Affiliates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AffiliateLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catagories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catagories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Affiliates_AffiliateLanguageId",
                table: "Affiliates",
                column: "AffiliateLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Affiliates_CatagoryId",
                table: "Affiliates",
                column: "CatagoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Affiliates_AffiliateLanguages_AffiliateLanguageId",
                table: "Affiliates",
                column: "AffiliateLanguageId",
                principalTable: "AffiliateLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Affiliates_Catagories_CatagoryId",
                table: "Affiliates",
                column: "CatagoryId",
                principalTable: "Catagories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Affiliates_AffiliateLanguages_AffiliateLanguageId",
                table: "Affiliates");

            migrationBuilder.DropForeignKey(
                name: "FK_Affiliates_Catagories_CatagoryId",
                table: "Affiliates");

            migrationBuilder.DropTable(
                name: "AffiliateLanguages");

            migrationBuilder.DropTable(
                name: "Catagories");

            migrationBuilder.DropIndex(
                name: "IX_Affiliates_AffiliateLanguageId",
                table: "Affiliates");

            migrationBuilder.DropIndex(
                name: "IX_Affiliates_CatagoryId",
                table: "Affiliates");

            migrationBuilder.DropColumn(
                name: "BitlyLink",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "AffiliateLanguageId",
                table: "Affiliates");

            migrationBuilder.DropColumn(
                name: "CatagoryId",
                table: "Affiliates");
        }
    }
}
