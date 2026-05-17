using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventra.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRelatedEventId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedEventId",
                table: "Notifications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedEventId",
                table: "Notifications");
        }
    }
}
