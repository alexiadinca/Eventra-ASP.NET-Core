using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventra.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteCountToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteCount",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "Events");
        }
    }
}
