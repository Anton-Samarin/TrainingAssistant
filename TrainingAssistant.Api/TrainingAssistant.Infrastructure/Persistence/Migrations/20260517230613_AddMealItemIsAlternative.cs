using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingAssistant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMealItemIsAlternative : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlternative",
                table: "MealItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlternative",
                table: "MealItems");
        }
    }
}
