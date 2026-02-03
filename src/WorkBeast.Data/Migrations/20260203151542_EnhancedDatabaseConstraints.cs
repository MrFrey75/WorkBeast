using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBeast.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedDatabaseConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSets_LoggedExerciseOid",
                table: "WorkoutSets");

            migrationBuilder.DropIndex(
                name: "IX_LoggedExercises_WorkoutSessionOid",
                table: "LoggedExercises");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_LoggedExerciseOid_SortOrder",
                table: "WorkoutSets",
                columns: new[] { "LoggedExerciseOid", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_IsDeleted",
                table: "WorkoutSessions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LoggedExercises_WorkoutSessionOid_Sequence",
                table: "LoggedExercises",
                columns: new[] { "WorkoutSessionOid", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_IsDeleted",
                table: "Exercises",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_BodyParts_Name",
                table: "BodyParts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedAt",
                table: "AspNetUsers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSets_LoggedExerciseOid_SortOrder",
                table: "WorkoutSets");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_IsDeleted",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_LoggedExercises_WorkoutSessionOid_Sequence",
                table: "LoggedExercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_IsDeleted",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_BodyParts_Name",
                table: "BodyParts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_LoggedExerciseOid",
                table: "WorkoutSets",
                column: "LoggedExerciseOid");

            migrationBuilder.CreateIndex(
                name: "IX_LoggedExercises_WorkoutSessionOid",
                table: "LoggedExercises",
                column: "WorkoutSessionOid");
        }
    }
}
