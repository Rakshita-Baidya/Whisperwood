using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisperwood.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bill");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Bill",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
