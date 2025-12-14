using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTasksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Description", "DueDate", "EndDate", "Name", "Priority", "StartDate", "Status" },
                values: new object[,]
                {
                    { 1, "Complete the documentation for the project, including all modules and APIs.", new DateTime(2025, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Complete Project Documentation", 0, new DateTime(2025, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Prepare onboarding materials and sessions for new developers joining the team.", new DateTime(2025, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Onboarding New Developers", 1, null, 0 },
                    { 3, "Updated the API documentation for version 2.0.", new DateTime(2025, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Finished: Update Documentation", 2, null, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
