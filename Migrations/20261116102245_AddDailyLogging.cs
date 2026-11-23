using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace progressa.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SleepLogs_UserId",
                table: "SleepLogs");

            migrationBuilder.DropIndex(
                name: "IX_NutritionLogs_UserId",
                table: "NutritionLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "WakeTime",
                table: "SleepLogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "SleepHours",
                table: "SleepLogs",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "SleepLogs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BedTime",
                table: "SleepLogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "GoalReached",
                table: "SleepLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ReachedProtein",
                table: "NutritionLogs",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReachedFat",
                table: "NutritionLogs",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReachedCarbs",
                table: "NutritionLogs",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReachedCalories",
                table: "NutritionLogs",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "NutritionLogs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "CaloriesGoalReached",
                table: "NutritionLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CarbsGoalReached",
                table: "NutritionLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FatGoalReached",
                table: "NutritionLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProteinGoalReached",
                table: "NutritionLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "NutritionLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SleepLogs_UserId_Date",
                table: "SleepLogs",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionLogs_UserId_Date",
                table: "NutritionLogs",
                columns: new[] { "UserId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SleepLogs_UserId_Date",
                table: "SleepLogs");

            migrationBuilder.DropIndex(
                name: "IX_NutritionLogs_UserId_Date",
                table: "NutritionLogs");

            migrationBuilder.DropColumn(
                name: "GoalReached",
                table: "SleepLogs");

            migrationBuilder.DropColumn(
                name: "CaloriesGoalReached",
                table: "NutritionLogs");

            migrationBuilder.DropColumn(
                name: "CarbsGoalReached",
                table: "NutritionLogs");

            migrationBuilder.DropColumn(
                name: "FatGoalReached",
                table: "NutritionLogs");

            migrationBuilder.DropColumn(
                name: "ProteinGoalReached",
                table: "NutritionLogs");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "NutritionLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "WakeTime",
                table: "SleepLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "SleepHours",
                table: "SleepLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "SleepLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BedTime",
                table: "SleepLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ReachedProtein",
                table: "NutritionLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ReachedFat",
                table: "NutritionLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ReachedCarbs",
                table: "NutritionLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReachedCalories",
                table: "NutritionLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "NutritionLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_SleepLogs_UserId",
                table: "SleepLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionLogs_UserId",
                table: "NutritionLogs",
                column: "UserId");
        }
    }
}
