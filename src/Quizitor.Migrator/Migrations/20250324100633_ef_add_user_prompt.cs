using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_add_user_prompt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "prompt_subject",
                table: "user");

            migrationBuilder.DropColumn(
                name: "prompt_type",
                table: "user");

            migrationBuilder.CreateTable(
                name: "user_prompt",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    bot_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    subject = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_prompt", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_prompt_bot_bot_id",
                        column: x => x.bot_id,
                        principalTable: "bot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_prompt_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_prompt_bot_id",
                table: "user_prompt",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_prompt_user_id_bot_id",
                table: "user_prompt",
                columns: new[] { "user_id", "bot_id" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_prompt");

            migrationBuilder.AddColumn<string>(
                name: "prompt_subject",
                table: "user",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prompt_type",
                table: "user",
                type: "integer",
                nullable: true);
        }
    }
}
