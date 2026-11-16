using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace progressa.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalMetricAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Metric",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_CategoryId",
                table: "Goals",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Categories_CategoryId",
                table: "Goals",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Categories_CategoryId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_CategoryId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Metric",
                table: "Goals");
        }
    }
}
