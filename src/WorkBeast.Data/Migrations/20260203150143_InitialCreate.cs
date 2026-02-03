using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBeast.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyParts",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyParts", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "BodyPartExercise",
                columns: table => new
                {
                    ExercisesOid = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetedBodyPartsOid = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyPartExercise", x => new { x.ExercisesOid, x.TargetedBodyPartsOid });
                    table.ForeignKey(
                        name: "FK_BodyPartExercise_BodyParts_TargetedBodyPartsOid",
                        column: x => x.TargetedBodyPartsOid,
                        principalTable: "BodyParts",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BodyPartExercise_Exercises_ExercisesOid",
                        column: x => x.ExercisesOid,
                        principalTable: "Exercises",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseBodyPart",
                columns: table => new
                {
                    ExerciseOid = table.Column<int>(type: "INTEGER", nullable: false),
                    BodyPartOid = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseBodyPart", x => new { x.ExerciseOid, x.BodyPartOid });
                    table.ForeignKey(
                        name: "FK_ExerciseBodyPart_BodyParts_BodyPartOid",
                        column: x => x.BodyPartOid,
                        principalTable: "BodyParts",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseBodyPart_Exercises_ExerciseOid",
                        column: x => x.ExerciseOid,
                        principalTable: "Exercises",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoggedExercises",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutSessionOid = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseOid = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoggedExercises", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_LoggedExercises_Exercises_ExerciseOid",
                        column: x => x.ExerciseOid,
                        principalTable: "Exercises",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoggedExercises_WorkoutSessions_WorkoutSessionOid",
                        column: x => x.WorkoutSessionOid,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSets",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoggedExerciseOid = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    ResistanceLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    SetType = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSets", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_WorkoutSets_LoggedExercises_LoggedExerciseOid",
                        column: x => x.LoggedExerciseOid,
                        principalTable: "LoggedExercises",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyPartExercise_TargetedBodyPartsOid",
                table: "BodyPartExercise",
                column: "TargetedBodyPartsOid");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseBodyPart_BodyPartOid",
                table: "ExerciseBodyPart",
                column: "BodyPartOid");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Name",
                table: "Exercises",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LoggedExercises_ExerciseOid",
                table: "LoggedExercises",
                column: "ExerciseOid");

            migrationBuilder.CreateIndex(
                name: "IX_LoggedExercises_Sequence",
                table: "LoggedExercises",
                column: "Sequence");

            migrationBuilder.CreateIndex(
                name: "IX_LoggedExercises_WorkoutSessionOid",
                table: "LoggedExercises",
                column: "WorkoutSessionOid");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_Date",
                table: "WorkoutSessions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_LoggedExerciseOid",
                table: "WorkoutSets",
                column: "LoggedExerciseOid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyPartExercise");

            migrationBuilder.DropTable(
                name: "ExerciseBodyPart");

            migrationBuilder.DropTable(
                name: "WorkoutSets");

            migrationBuilder.DropTable(
                name: "BodyParts");

            migrationBuilder.DropTable(
                name: "LoggedExercises");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");
        }
    }
}
