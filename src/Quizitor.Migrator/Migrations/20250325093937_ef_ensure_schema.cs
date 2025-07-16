using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_ensure_schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "user_role",
                newName: "user_role",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "user_prompt",
                newName: "user_prompt",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "user_permission",
                newName: "user_permission",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "user",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "team_member",
                newName: "team_member",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "team_leader",
                newName: "team_leader",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "team",
                newName: "team",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "submission",
                newName: "submission",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "session",
                newName: "session",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "round",
                newName: "round",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "role_permission",
                newName: "role_permission",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "role",
                newName: "role",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "question_timing",
                newName: "question_timing",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "question_rule",
                newName: "question_rule",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "question_option",
                newName: "question_option",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "question",
                newName: "question",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "mailing",
                newName: "mailing",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "game",
                newName: "game",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "bot_interaction",
                newName: "bot_interaction",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "bot_command",
                newName: "bot_command",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "bot",
                newName: "bot",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "user_role",
                schema: "public",
                newName: "user_role");

            migrationBuilder.RenameTable(
                name: "user_prompt",
                schema: "public",
                newName: "user_prompt");

            migrationBuilder.RenameTable(
                name: "user_permission",
                schema: "public",
                newName: "user_permission");

            migrationBuilder.RenameTable(
                name: "user",
                schema: "public",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "team_member",
                schema: "public",
                newName: "team_member");

            migrationBuilder.RenameTable(
                name: "team_leader",
                schema: "public",
                newName: "team_leader");

            migrationBuilder.RenameTable(
                name: "team",
                schema: "public",
                newName: "team");

            migrationBuilder.RenameTable(
                name: "submission",
                schema: "public",
                newName: "submission");

            migrationBuilder.RenameTable(
                name: "session",
                schema: "public",
                newName: "session");

            migrationBuilder.RenameTable(
                name: "round",
                schema: "public",
                newName: "round");

            migrationBuilder.RenameTable(
                name: "role_permission",
                schema: "public",
                newName: "role_permission");

            migrationBuilder.RenameTable(
                name: "role",
                schema: "public",
                newName: "role");

            migrationBuilder.RenameTable(
                name: "question_timing",
                schema: "public",
                newName: "question_timing");

            migrationBuilder.RenameTable(
                name: "question_rule",
                schema: "public",
                newName: "question_rule");

            migrationBuilder.RenameTable(
                name: "question_option",
                schema: "public",
                newName: "question_option");

            migrationBuilder.RenameTable(
                name: "question",
                schema: "public",
                newName: "question");

            migrationBuilder.RenameTable(
                name: "mailing",
                schema: "public",
                newName: "mailing");

            migrationBuilder.RenameTable(
                name: "game",
                schema: "public",
                newName: "game");

            migrationBuilder.RenameTable(
                name: "bot_interaction",
                schema: "public",
                newName: "bot_interaction");

            migrationBuilder.RenameTable(
                name: "bot_command",
                schema: "public",
                newName: "bot_command");

            migrationBuilder.RenameTable(
                name: "bot",
                schema: "public",
                newName: "bot");
        }
    }
}
