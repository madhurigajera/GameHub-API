using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteReleaseDateToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseNote",
                table: "Games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReleaseNote",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
