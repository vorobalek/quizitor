using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_add_mailing_profile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mailing_profile",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mailing_id = table.Column<int>(type: "integer", nullable: false),
                    owner_id = table.Column<long>(type: "bigint", nullable: false),
                    contact_type = table.Column<int>(type: "integer", nullable: false),
                    bot_types = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mailing_profile", x => x.id);
                    table.ForeignKey(
                        name: "fk_mailing_profile_mailing_mailing_id",
                        column: x => x.mailing_id,
                        principalSchema: "public",
                        principalTable: "mailing",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mailing_profile_user_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mailing_filter_bot",
                schema: "public",
                columns: table => new
                {
                    mailing_profile_id = table.Column<int>(type: "integer", nullable: false),
                    bot_id = table.Column<int>(type: "integer", nullable: false),
                    flag_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mailing_filter_bot", x => new { x.mailing_profile_id, x.bot_id });
                    table.ForeignKey(
                        name: "fk_mailing_filter_bot_bot_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "public",
                        principalTable: "bot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mailing_filter_bot_mailing_profile_mailing_profile_id",
                        column: x => x.mailing_profile_id,
                        principalSchema: "public",
                        principalTable: "mailing_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mailing_filter_game",
                schema: "public",
                columns: table => new
                {
                    mailing_profile_id = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    flag_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mailing_filter_game", x => new { x.mailing_profile_id, x.game_id });
                    table.ForeignKey(
                        name: "fk_mailing_filter_game_game_game_id",
                        column: x => x.game_id,
                        principalSchema: "public",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mailing_filter_game_mailing_profile_mailing_profile_id",
                        column: x => x.mailing_profile_id,
                        principalSchema: "public",
                        principalTable: "mailing_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mailing_filter_session",
                schema: "public",
                columns: table => new
                {
                    mailing_profile_id = table.Column<int>(type: "integer", nullable: false),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    flag_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mailing_filter_session", x => new { x.mailing_profile_id, x.session_id });
                    table.ForeignKey(
                        name: "fk_mailing_filter_session_mailing_profile_mailing_profile_id",
                        column: x => x.mailing_profile_id,
                        principalSchema: "public",
                        principalTable: "mailing_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mailing_filter_session_session_session_id",
                        column: x => x.session_id,
                        principalSchema: "public",
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mailing_filter_team",
                schema: "public",
                columns: table => new
                {
                    mailing_profile_id = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    flag_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mailing_filter_team", x => new { x.mailing_profile_id, x.team_id });
                    table.ForeignKey(
                        name: "fk_mailing_filter_team_mailing_profile_mailing_profile_id",
                        column: x => x.mailing_profile_id,
                        principalSchema: "public",
                        principalTable: "mailing_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mailing_filter_team_team_team_id",
                        column: x => x.team_id,
                        principalSchema: "public",
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mailing_filter_user",
                schema: "public",
                columns: table => new
                {
                    mailing_profile_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    flag_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mailing_filter_user", x => new { x.mailing_profile_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_mailing_filter_user_mailing_profile_mailing_profile_id",
                        column: x => x.mailing_profile_id,
                        principalSchema: "public",
                        principalTable: "mailing_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mailing_filter_user_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_mailing_filter_bot_bot_id",
                schema: "public",
                table: "mailing_filter_bot",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_mailing_filter_game_game_id",
                schema: "public",
                table: "mailing_filter_game",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_mailing_filter_session_session_id",
                schema: "public",
                table: "mailing_filter_session",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_mailing_filter_team_team_id",
                schema: "public",
                table: "mailing_filter_team",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_mailing_filter_user_user_id",
                schema: "public",
                table: "mailing_filter_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_mailing_profile_mailing_id_owner_id",
                schema: "public",
                table: "mailing_profile",
                columns: new[] { "mailing_id", "owner_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mailing_profile_owner_id",
                schema: "public",
                table: "mailing_profile",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mailing_filter_bot",
                schema: "public");

            migrationBuilder.DropTable(
                name: "mailing_filter_game",
                schema: "public");

            migrationBuilder.DropTable(
                name: "mailing_filter_session",
                schema: "public");

            migrationBuilder.DropTable(
                name: "mailing_filter_team",
                schema: "public");

            migrationBuilder.DropTable(
                name: "mailing_filter_user",
                schema: "public");

            migrationBuilder.DropTable(
                name: "mailing_profile",
                schema: "public");
        }
    }
}
