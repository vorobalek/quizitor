using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_question_add_attempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "attempts",
                table: "question",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attempts",
                table: "question");
        }
    }
}
