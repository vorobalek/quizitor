using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_add_team_member : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "team_member",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_member", x => new { x.team_id, x.session_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_team_member_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_member_team_team_id",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_member_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_team_member_session_id",
                table: "team_member",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_member_user_id",
                table: "team_member",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "team_member");
        }
    }
}
