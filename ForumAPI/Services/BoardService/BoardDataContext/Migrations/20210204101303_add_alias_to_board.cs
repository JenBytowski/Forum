using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumAPI.Services.BoardService.BoardDataContext.Migrations
{
    public partial class add_alias_to_board : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Boards",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Boards");
        }
    }
}
