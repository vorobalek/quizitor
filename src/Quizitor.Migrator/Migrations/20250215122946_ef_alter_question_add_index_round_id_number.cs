using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_question_add_index_round_id_number : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_question_round_id",
                table: "question");

            migrationBuilder.CreateIndex(
                name: "ix_question_round_id_number",
                table: "question",
                columns: new[] { "round_id", "number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_question_round_id_number",
                table: "question");

            migrationBuilder.CreateIndex(
                name: "ix_question_round_id",
                table: "question",
                column: "round_id");
        }
    }
}
