using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace progressa.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettingsPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistanceUnit",
                table: "UserSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "km");

            migrationBuilder.AddColumn<string>(
                name: "EnergyUnit",
                table: "UserSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "kcal");

            migrationBuilder.AddColumn<string>(
                name: "HeightUnit",
                table: "UserSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "cm");

            migrationBuilder.AddColumn<string>(
                name: "TimeFormat",
                table: "UserSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "24h");

            migrationBuilder.AddColumn<string>(
                name: "WeekStart",
                table: "UserSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "mon");

            migrationBuilder.AddColumn<string>(
                name: "WeightUnit",
                table: "UserSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "kg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceUnit",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "EnergyUnit",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "HeightUnit",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "TimeFormat",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "WeekStart",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "UserSettings");
        }
    }
}
