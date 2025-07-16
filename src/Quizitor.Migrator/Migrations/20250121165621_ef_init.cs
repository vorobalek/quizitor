using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "events");

            migrationBuilder.CreateTable(
                name: "bot",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    username = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    drop_pending_updates = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bot", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bot_command",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bot_type = table.Column<int>(type: "integer", nullable: false),
                    command = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bot_command", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    system_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "round",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_round", x => x.id);
                    table.ForeignKey(
                        name: "fk_round_game_game_id",
                        column: x => x.game_id,
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_session_game_game_id",
                        column: x => x.game_id,
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    system_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_permission_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    text = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    comment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    time = table.Column<int>(type: "integer", nullable: false),
                    notification_time = table.Column<int>(type: "integer", nullable: true),
                    auto_close = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_round_round_id",
                        column: x => x.round_id,
                        principalTable: "round",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    last_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    username = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    game_server_id = table.Column<int>(type: "integer", nullable: true),
                    game_admin_id = table.Column<int>(type: "integer", nullable: true),
                    session_id = table.Column<int>(type: "integer", nullable: true),
                    prompt_type = table.Column<int>(type: "integer", nullable: true),
                    prompt_subject = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_bot_game_admin_id",
                        column: x => x.game_admin_id,
                        principalTable: "bot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_bot_game_server_id",
                        column: x => x.game_server_id,
                        principalTable: "bot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_option", x => x.id);
                    table.ForeignKey(
                        name: "fk_option_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "timing",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    stop_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timing", x => x.id);
                    table.ForeignKey(
                        name: "fk_timing_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_timing_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bot_interaction",
                columns: table => new
                {
                    bot_username = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    last_interaction = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bot_interaction", x => new { x.bot_username, x.user_id });
                    table.ForeignKey(
                        name: "fk_bot_interaction_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submission",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    time = table.Column<int>(type: "integer", nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    checking_result = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_submission", x => x.id);
                    table.ForeignKey(
                        name: "fk_submission_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_submission_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_submission_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    system_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_permission_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_user_role_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_role_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "timing_notify",
                schema: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timing_id = table.Column<int>(type: "integer", nullable: false),
                    run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timing_notify", x => x.id);
                    table.ForeignKey(
                        name: "fk_timing_notify_timing_timing_id",
                        column: x => x.timing_id,
                        principalTable: "timing",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "timing_stop",
                schema: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timing_id = table.Column<int>(type: "integer", nullable: false),
                    run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    round_page_number = table.Column<int>(type: "integer", nullable: false),
                    question_page_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timing_stop", x => x.id);
                    table.ForeignKey(
                        name: "fk_timing_stop_timing_timing_id",
                        column: x => x.timing_id,
                        principalTable: "timing",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bot_command_bot_type_command",
                table: "bot_command",
                columns: new[] { "bot_type", "command" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bot_interaction_user_id",
                table: "bot_interaction",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_option_question_id",
                table: "option",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_round_id",
                table: "question",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_system_name",
                table: "role",
                column: "system_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_role_id",
                table: "role_permission",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_round_game_id",
                table: "round",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_game_id",
                table: "session",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_submission_question_id",
                table: "submission",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_submission_session_id",
                table: "submission",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_submission_user_id",
                table: "submission",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_timing_question_id",
                table: "timing",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_timing_session_id",
                table: "timing",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_timing_notify_timing_id",
                schema: "events",
                table: "timing_notify",
                column: "timing_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_timing_stop_timing_id",
                schema: "events",
                table: "timing_stop",
                column: "timing_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_game_admin_id",
                table: "user",
                column: "game_admin_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_server_id",
                table: "user",
                column: "game_server_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_session_id",
                table: "user",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_permission_user_id",
                table: "user_permission",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_user_id",
                table: "user_role",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bot_command");

            migrationBuilder.DropTable(
                name: "bot_interaction");

            migrationBuilder.DropTable(
                name: "option");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "submission");

            migrationBuilder.DropTable(
                name: "timing_notify",
                schema: "events");

            migrationBuilder.DropTable(
                name: "timing_stop",
                schema: "events");

            migrationBuilder.DropTable(
                name: "user_permission");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "timing");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "bot");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "round");

            migrationBuilder.DropTable(
                name: "game");
        }
    }
}
