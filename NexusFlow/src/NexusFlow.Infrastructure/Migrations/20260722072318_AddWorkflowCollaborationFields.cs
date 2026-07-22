using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowCollaborationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignmentNote",
                table: "ProjectTasks",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelatedTaskId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedTaskId",
                table: "Notifications",
                column: "RelatedTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProjectTasks_RelatedTaskId",
                table: "Notifications",
                column: "RelatedTaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProjectTasks_RelatedTaskId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RelatedTaskId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AssignmentNote",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "RelatedTaskId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Comments");
        }
    }
}
