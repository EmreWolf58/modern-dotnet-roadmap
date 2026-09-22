using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Description", "IsCompleted", "IsDeleted", "Priority", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Learn Entity Framework Core basics", false, false, 1, "Learn EF Core" },
                    { 2, new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Practice EF Core migrations", false, false, 2, "Learn Migrations" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
