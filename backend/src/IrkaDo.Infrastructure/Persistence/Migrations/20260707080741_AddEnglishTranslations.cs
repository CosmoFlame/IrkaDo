using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IrkaDo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnglishTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaptionEn",
                table: "TravelHighlights",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationEn",
                table: "TravelHighlights",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContinentEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsIncludedEn",
                table: "TravelGuides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Tags",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "SocialLinks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionEn",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleEn",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "NewsArticles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExcerptEn",
                table: "NewsArticles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionEn",
                table: "NewsArticles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleEn",
                table: "NewsArticles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "NewsArticles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltTextEn",
                table: "MediaAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyEn",
                table: "HomeSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentJsonEn",
                table: "HomeSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadlineEn",
                table: "HomeSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Collaborations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestimonialEn",
                table: "Collaborations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Categories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaptionEn",
                table: "TravelHighlights");

            migrationBuilder.DropColumn(
                name: "DestinationEn",
                table: "TravelHighlights");

            migrationBuilder.DropColumn(
                name: "CityEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "ContinentEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "CountryEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "MetaTitleEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "WhatsIncludedEn",
                table: "TravelGuides");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "SocialLinks");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionEn",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "MetaTitleEn",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "ExcerptEn",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionEn",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "MetaTitleEn",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "AltTextEn",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "BodyEn",
                table: "HomeSections");

            migrationBuilder.DropColumn(
                name: "ContentJsonEn",
                table: "HomeSections");

            migrationBuilder.DropColumn(
                name: "HeadlineEn",
                table: "HomeSections");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Collaborations");

            migrationBuilder.DropColumn(
                name: "TestimonialEn",
                table: "Collaborations");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Categories");
        }
    }
}
