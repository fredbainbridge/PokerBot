using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PokerBot.Migrations
{
    public partial class HandDateTime2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Hand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Hand");
        }
    }
}
