using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_rename_timing_stop_to_question_timing_stop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_timing_stop_question_timing_timing_id",
                schema: "events",
                table: "timing_stop");

            migrationBuilder.DropPrimaryKey(
                name: "pk_timing_stop",
                schema: "events",
                table: "timing_stop");

            migrationBuilder.RenameTable(
                name: "timing_stop",
                schema: "events",
                newName: "question_timing_stop",
                newSchema: "events");

            migrationBuilder.RenameIndex(
                name: "ix_timing_stop_timing_id",
                schema: "events",
                table: "question_timing_stop",
                newName: "ix_question_timing_stop_timing_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_timing_stop",
                schema: "events",
                table: "question_timing_stop",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_timing_stop_question_timing_timing_id",
                schema: "events",
                table: "question_timing_stop",
                column: "timing_id",
                principalTable: "question_timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_timing_stop_question_timing_timing_id",
                schema: "events",
                table: "question_timing_stop");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_timing_stop",
                schema: "events",
                table: "question_timing_stop");

            migrationBuilder.RenameTable(
                name: "question_timing_stop",
                schema: "events",
                newName: "timing_stop",
                newSchema: "events");

            migrationBuilder.RenameIndex(
                name: "ix_question_timing_stop_timing_id",
                schema: "events",
                table: "timing_stop",
                newName: "ix_timing_stop_timing_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_timing_stop",
                schema: "events",
                table: "timing_stop",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_timing_stop_question_timing_timing_id",
                schema: "events",
                table: "timing_stop",
                column: "timing_id",
                principalTable: "question_timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
