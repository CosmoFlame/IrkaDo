using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IrkaDo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHighlightsContinentAddContentLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborations_MediaAssets_LogoId",
                table: "Collaborations");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_Collaborations_CollaborationId",
                table: "MediaAssets");

            migrationBuilder.DropTable(
                name: "TravelHighlights");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_CollaborationId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "Continent",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "ContinentEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "CollaborationId",
                table: "MediaAssets");

            migrationBuilder.RenameColumn(
                name: "LogoId",
                table: "Collaborations",
                newName: "CoverImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Collaborations_LogoId",
                table: "Collaborations",
                newName: "IX_Collaborations_CoverImageId");

            migrationBuilder.CreateTable(
                name: "ContentLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    TitleEn = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    NewsArticleId = table.Column<Guid>(type: "uuid", nullable: true),
                    TravelGuideId = table.Column<Guid>(type: "uuid", nullable: true),
                    CollaborationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentLinks_Collaborations_CollaborationId",
                        column: x => x.CollaborationId,
                        principalTable: "Collaborations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentLinks_NewsArticles_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentLinks_TravelGuides_TravelGuideId",
                        column: x => x.TravelGuideId,
                        principalTable: "TravelGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentLinks_CollaborationId",
                table: "ContentLinks",
                column: "CollaborationId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLinks_NewsArticleId",
                table: "ContentLinks",
                column: "NewsArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLinks_TravelGuideId",
                table: "ContentLinks",
                column: "TravelGuideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborations_MediaAssets_CoverImageId",
                table: "Collaborations",
                column: "CoverImageId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborations_MediaAssets_CoverImageId",
                table: "Collaborations");

            migrationBuilder.DropTable(
                name: "ContentLinks");

            migrationBuilder.RenameColumn(
                name: "CoverImageId",
                table: "Collaborations",
                newName: "LogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Collaborations_CoverImageId",
                table: "Collaborations",
                newName: "IX_Collaborations_LogoId");

            migrationBuilder.AddColumn<string>(
                name: "Continent",
                table: "TravelGuides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContinentEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CollaborationId",
                table: "MediaAssets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TravelHighlights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: false),
                    CaptionEn = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    DestinationEn = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelHighlights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelHighlights_MediaAssets_ImageId",
                        column: x => x.ImageId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_CollaborationId",
                table: "MediaAssets",
                column: "CollaborationId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelHighlights_ImageId",
                table: "TravelHighlights",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborations_MediaAssets_LogoId",
                table: "Collaborations",
                column: "LogoId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_Collaborations_CollaborationId",
                table: "MediaAssets",
                column: "CollaborationId",
                principalTable: "Collaborations",
                principalColumn: "Id");
        }
    }
}
