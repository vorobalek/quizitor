using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_question_option_drop_is_correct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update public.question_option set cost = 1 where is_correct is true and cost = 0");
            
            migrationBuilder.DropColumn(
                name: "is_correct",
                schema: "public",
                table: "question_option");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_correct",
                schema: "public",
                table: "question_option",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
