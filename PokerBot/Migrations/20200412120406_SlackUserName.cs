using Microsoft.EntityFrameworkCore.Migrations;

namespace PokerBot.Migrations
{
    public partial class SlackUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SlackUserName",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlackUserName",
                table: "User");
        }
    }
}
