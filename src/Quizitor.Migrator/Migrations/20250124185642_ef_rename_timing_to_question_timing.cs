using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_rename_timing_to_question_timing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_timing_question_question_id",
                table: "timing");

            migrationBuilder.DropForeignKey(
                name: "fk_timing_session_session_id",
                table: "timing");

            migrationBuilder.DropForeignKey(
                name: "fk_timing_notify_timing_timing_id",
                schema: "events",
                table: "timing_notify");

            migrationBuilder.DropForeignKey(
                name: "fk_timing_stop_timing_timing_id",
                schema: "events",
                table: "timing_stop");

            migrationBuilder.DropPrimaryKey(
                name: "pk_timing",
                table: "timing");

            migrationBuilder.RenameTable(
                name: "timing",
                newName: "question_timing");

            migrationBuilder.RenameIndex(
                name: "ix_timing_session_id",
                table: "question_timing",
                newName: "ix_question_timing_session_id");

            migrationBuilder.RenameIndex(
                name: "ix_timing_question_id",
                table: "question_timing",
                newName: "ix_question_timing_question_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_timing",
                table: "question_timing",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_timing_question_question_id",
                table: "question_timing",
                column: "question_id",
                principalTable: "question",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_question_timing_session_session_id",
                table: "question_timing",
                column: "session_id",
                principalTable: "session",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_timing_notify_question_timing_timing_id",
                schema: "events",
                table: "timing_notify",
                column: "timing_id",
                principalTable: "question_timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_timing_stop_question_timing_timing_id",
                schema: "events",
                table: "timing_stop",
                column: "timing_id",
                principalTable: "question_timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_timing_question_question_id",
                table: "question_timing");

            migrationBuilder.DropForeignKey(
                name: "fk_question_timing_session_session_id",
                table: "question_timing");

            migrationBuilder.DropForeignKey(
                name: "fk_timing_notify_question_timing_timing_id",
                schema: "events",
                table: "timing_notify");

            migrationBuilder.DropForeignKey(
                name: "fk_timing_stop_question_timing_timing_id",
                schema: "events",
                table: "timing_stop");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_timing",
                table: "question_timing");

            migrationBuilder.RenameTable(
                name: "question_timing",
                newName: "timing");

            migrationBuilder.RenameIndex(
                name: "ix_question_timing_session_id",
                table: "timing",
                newName: "ix_timing_session_id");

            migrationBuilder.RenameIndex(
                name: "ix_question_timing_question_id",
                table: "timing",
                newName: "ix_timing_question_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_timing",
                table: "timing",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_timing_question_question_id",
                table: "timing",
                column: "question_id",
                principalTable: "question",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_timing_session_session_id",
                table: "timing",
                column: "session_id",
                principalTable: "session",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_timing_notify_timing_timing_id",
                schema: "events",
                table: "timing_notify",
                column: "timing_id",
                principalTable: "timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_timing_stop_timing_timing_id",
                schema: "events",
                table: "timing_stop",
                column: "timing_id",
                principalTable: "timing",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
