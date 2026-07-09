using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IrkaDo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContactEmailToHomeSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "HomeSections",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "HomeSections");
        }
    }
}
