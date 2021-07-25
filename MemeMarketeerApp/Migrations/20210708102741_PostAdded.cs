using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MemeMarketeerApp.Migrations
{
    public partial class PostAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PostTypeId = table.Column<int>(nullable: false),
                    AffiliateId = table.Column<int>(nullable: false),
                    CampaignId = table.Column<int>(nullable: false),
                    PostReached = table.Column<double>(nullable: false),
                    StoryViewed = table.Column<double>(nullable: false),
                    ImpressionGenerated = table.Column<double>(nullable: false),
                    ReactionGenerated = table.Column<double>(nullable: false),
                    CommentsGenerated = table.Column<double>(nullable: false),
                    PaidAmount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalTable: "Affiliates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_PostTypes_PostTypeId",
                        column: x => x.PostTypeId,
                        principalTable: "PostTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PostTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Post Only" });

            migrationBuilder.InsertData(
                table: "PostTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Story Only" });

            migrationBuilder.InsertData(
                table: "PostTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Post + Story" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AffiliateId",
                table: "Posts",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CampaignId",
                table: "Posts",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PostTypeId",
                table: "Posts",
                column: "PostTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "PostTypes");
        }
    }
}
