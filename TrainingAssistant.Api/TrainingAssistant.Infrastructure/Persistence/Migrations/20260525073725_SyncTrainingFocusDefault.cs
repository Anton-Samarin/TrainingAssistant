using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingAssistant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncTrainingFocusDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrainingFocus",
                table: "UserProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer");

            // Профили, созданные до defaultValue=2, получили 0 (Strength) при AddPlanEnhancements
            migrationBuilder.Sql(
                """UPDATE "UserProfiles" SET "TrainingFocus" = 2 WHERE "TrainingFocus" = 0;""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrainingFocus",
                table: "UserProfiles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);
        }
    }
}
