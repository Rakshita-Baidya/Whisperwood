using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisperwood.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DiscountCodes_DiscountCodeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Users_UsersId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_UsersId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DiscountCodeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DiscountCodeId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Orders",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderedAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "Books",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => new { x.OrderId, x.BookId });
                    table.ForeignKey(
                        name: "FK_OrderItem_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_UserId",
                table: "Promotions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderedAt",
                table: "Orders",
                column: "OrderedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_BookId",
                table: "OrderItem",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Users_UserId",
                table: "Promotions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Users_UserId",
                table: "Promotions");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_UserId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedAt",
                table: "Orders");

            migrationBuilder.AddColumn<Guid>(
                name: "UsersId",
                table: "Promotions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountCodeId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Format",
                table: "Books",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_UsersId",
                table: "Promotions",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DiscountCodeId",
                table: "Orders",
                column: "DiscountCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DiscountCodes_DiscountCodeId",
                table: "Orders",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Users_UsersId",
                table: "Promotions",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
