using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_rename_timing_notify_to_question_timing_notify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_timing_notify_question_timing_timing_id",
                schema: "events",
                table: "timing_notify");

            migrationBuilder.DropPrimaryKey(
                name: "pk_timing_notify",
                schema: "events",
                table: "timing_notify");

            migrationBuilder.RenameTable(
                name: "timing_notify",
                schema: "events",
                newName: "question_timing_notify",
                newSchema: "events");

            migrationBuilder.RenameIndex(
                name: "ix_timing_notify_timing_id",
                schema: "events",
                table: "question_timing_notify",
                newName: "ix_question_timing_notify_timing_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_timing_notify",
                schema: "events",
                table: "question_timing_notify",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_timing_notify_question_timing_timing_id",
                schema: "events",
                table: "question_timing_notify",
                column: "timing_id",
                principalTable: "question_timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_timing_notify_question_timing_timing_id",
                schema: "events",
                table: "question_timing_notify");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_timing_notify",
                schema: "events",
                table: "question_timing_notify");

            migrationBuilder.RenameTable(
                name: "question_timing_notify",
                schema: "events",
                newName: "timing_notify",
                newSchema: "events");

            migrationBuilder.RenameIndex(
                name: "ix_question_timing_notify_timing_id",
                schema: "events",
                table: "timing_notify",
                newName: "ix_timing_notify_timing_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_timing_notify",
                schema: "events",
                table: "timing_notify",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_timing_notify_question_timing_timing_id",
                schema: "events",
                table: "timing_notify",
                column: "timing_id",
                principalTable: "question_timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
