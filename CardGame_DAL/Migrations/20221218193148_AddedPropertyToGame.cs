using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGame_DAL.Migrations
{
    public partial class AddedPropertyToGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameName",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameName",
                table: "Games");
        }
    }
}
