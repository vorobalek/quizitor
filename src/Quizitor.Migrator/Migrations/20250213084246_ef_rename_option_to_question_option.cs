using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_rename_option_to_question_option : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_option_question_question_id",
                table: "option");

            migrationBuilder.DropPrimaryKey(
                name: "pk_option",
                table: "option");

            migrationBuilder.RenameTable(
                name: "option",
                newName: "question_option");

            migrationBuilder.RenameIndex(
                name: "ix_option_question_id",
                table: "question_option",
                newName: "ix_question_option_question_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_option",
                table: "question_option",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_option_question_question_id",
                table: "question_option",
                column: "question_id",
                principalTable: "question",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_option_question_question_id",
                table: "question_option");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_option",
                table: "question_option");

            migrationBuilder.RenameTable(
                name: "question_option",
                newName: "option");

            migrationBuilder.RenameIndex(
                name: "ix_question_option_question_id",
                table: "option",
                newName: "ix_option_question_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_option",
                table: "option",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_option_question_question_id",
                table: "option",
                column: "question_id",
                principalTable: "question",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
