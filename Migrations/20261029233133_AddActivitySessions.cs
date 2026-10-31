using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace progressa.Migrations
{
    /// <inheritdoc />
    public partial class AddActivitySessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaloriesBurned",
                table: "ActivitySessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ActivitySessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ActivitySessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "ActivitySessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ActivitySessions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ActivitySessions");
        }
    }
}
