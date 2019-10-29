using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokerBot.Migrations
{
    public partial class PaymentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayeeID = table.Column<int>(nullable: false),
                    PayerID = table.Column<int>(nullable: false),
                    DateRequested = table.Column<DateTime>(nullable: false),
                    Sent = table.Column<DateTime>(nullable: true),
                    Confirmed = table.Column<DateTime>(nullable: true),
                    Chips = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payment_User_PayeeID",
                        column: x => x.PayeeID,
                        principalTable: "User",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Payment_User_PayerID",
                        column: x => x.PayerID,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PayeeID",
                table: "Payment",
                column: "PayeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PayerID",
                table: "Payment",
                column: "PayerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");
        }
    }
}
