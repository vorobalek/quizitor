using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_team_leader_change_pk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_team_leader",
                schema: "public",
                table: "team_leader");

            migrationBuilder.AddPrimaryKey(
                name: "pk_team_leader",
                schema: "public",
                table: "team_leader",
                columns: new[] { "team_id", "session_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_team_leader",
                schema: "public",
                table: "team_leader");

            migrationBuilder.AddPrimaryKey(
                name: "pk_team_leader",
                schema: "public",
                table: "team_leader",
                columns: new[] { "team_id", "session_id", "user_id" });
        }
    }
}
