using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSvc.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixchecktask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "TodoItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "TodoItems");
        }
    }
}
