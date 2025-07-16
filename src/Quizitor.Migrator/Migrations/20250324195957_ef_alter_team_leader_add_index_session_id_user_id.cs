using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_team_leader_add_index_session_id_user_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_team_leader_session_id",
                table: "team_leader");

            migrationBuilder.CreateIndex(
                name: "ix_team_leader_session_id_user_id",
                table: "team_leader",
                columns: new[] { "session_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_team_leader_session_id_user_id",
                table: "team_leader");

            migrationBuilder.CreateIndex(
                name: "ix_team_leader_session_id",
                table: "team_leader",
                column: "session_id");
        }
    }
}
