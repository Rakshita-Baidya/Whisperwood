using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisperwood.Migrations
{
    /// <inheritdoc />
    public partial class updateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailablilityStatus",
                table: "Books",
                newName: "AvailabilityStatus");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AvailabilityStatus",
                table: "Books",
                newName: "AvailablilityStatus");
        }
    }
}
