using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingAssistant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrainingFocus",
                table: "UserProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TrainingExercises",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "MealItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserExercisePreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvoidExerciseName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    PreferredExerciseName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    PoolKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExercisePreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExercisePreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserExercisePreferences_UserId_AvoidExerciseName",
                table: "UserExercisePreferences",
                columns: new[] { "UserId", "AvoidExerciseName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserExercisePreferences");

            migrationBuilder.DropColumn(
                name: "TrainingFocus",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TrainingExercises");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "MealItems");
        }
    }
}
