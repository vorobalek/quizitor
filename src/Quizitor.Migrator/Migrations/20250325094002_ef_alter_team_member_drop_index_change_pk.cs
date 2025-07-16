using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_team_member_drop_index_change_pk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_team_member",
                schema: "public",
                table: "team_member");

            migrationBuilder.DropIndex(
                name: "ix_team_member_session_id_user_id",
                schema: "public",
                table: "team_member");

            migrationBuilder.DropIndex(
                name: "ix_team_member_user_id",
                schema: "public",
                table: "team_member");

            migrationBuilder.AddPrimaryKey(
                name: "pk_team_member",
                schema: "public",
                table: "team_member",
                columns: new[] { "user_id", "session_id" });

            migrationBuilder.CreateIndex(
                name: "ix_team_member_session_id",
                schema: "public",
                table: "team_member",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_member_team_id",
                schema: "public",
                table: "team_member",
                column: "team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_team_member",
                schema: "public",
                table: "team_member");

            migrationBuilder.DropIndex(
                name: "ix_team_member_session_id",
                schema: "public",
                table: "team_member");

            migrationBuilder.DropIndex(
                name: "ix_team_member_team_id",
                schema: "public",
                table: "team_member");

            migrationBuilder.AddPrimaryKey(
                name: "pk_team_member",
                schema: "public",
                table: "team_member",
                columns: new[] { "team_id", "session_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_team_member_session_id_user_id",
                schema: "public",
                table: "team_member",
                columns: new[] { "session_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_member_user_id",
                schema: "public",
                table: "team_member",
                column: "user_id");
        }
    }
}
