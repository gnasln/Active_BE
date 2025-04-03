using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSvc.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class resetdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompleteDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TodoItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TodoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompleteDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerName = table.Column<string>(type: "text", nullable: false),
                    Assigner = table.Column<Guid>(type: "uuid", nullable: false),
                    Assignee = table.Column<Guid>(type: "uuid", nullable: true),
                    AssigneeName = table.Column<string>(type: "text", nullable: false),
                    UnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentTodoItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_TodoItems_ParentTodoItemId",
                        column: x => x.ParentTodoItemId,
                        principalTable: "TodoItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TodoItemsComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    ParentTodoItemCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TodoItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItemsComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItemsComments_TodoItemsComments_ParentTodoItemCommentId",
                        column: x => x.ParentTodoItemCommentId,
                        principalTable: "TodoItemsComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TodoItemsComments_TodoItems_TodoItemId",
                        column: x => x.TodoItemId,
                        principalTable: "TodoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ParentTodoItemId",
                table: "TodoItems",
                column: "ParentTodoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemsComments_ParentTodoItemCommentId",
                table: "TodoItemsComments",
                column: "ParentTodoItemCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemsComments_TodoItemId",
                table: "TodoItemsComments",
                column: "TodoItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTasks");

            migrationBuilder.DropTable(
                name: "TodoHistories");

            migrationBuilder.DropTable(
                name: "TodoItemsComments");

            migrationBuilder.DropTable(
                name: "TodoItems");
        }
    }
}
