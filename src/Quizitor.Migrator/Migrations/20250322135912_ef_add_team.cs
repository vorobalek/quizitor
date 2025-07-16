using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_add_team : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "submission",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "team",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    owner_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_user_owner_id",
                        column: x => x.owner_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_submission_team_id",
                table: "submission",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_owner_id",
                table: "team",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_submission_team_team_id",
                table: "submission",
                column: "team_id",
                principalTable: "team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_submission_team_team_id",
                table: "submission");

            migrationBuilder.DropTable(
                name: "team");

            migrationBuilder.DropIndex(
                name: "ix_submission_team_id",
                table: "submission");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "submission");
        }
    }
}
