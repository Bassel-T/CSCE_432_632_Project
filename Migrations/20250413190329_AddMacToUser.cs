using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSCE_432_632_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddMacToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mac",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mac",
                table: "Users");
        }
    }
}
