using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisperwood.Migrations
{
    /// <inheritdoc />
    public partial class RemovePromotionBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionBook");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionBook",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionBook", x => new { x.BookId, x.PromotionId });
                    table.ForeignKey(
                        name: "FK_PromotionBook_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionBook_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionBook_PromotionId",
                table: "PromotionBook",
                column: "PromotionId");
        }
    }
}
