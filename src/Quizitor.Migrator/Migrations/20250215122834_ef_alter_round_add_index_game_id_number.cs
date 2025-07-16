using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizitor.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ef_alter_round_add_index_game_id_number : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_round_game_id",
                table: "round");

            migrationBuilder.CreateIndex(
                name: "ix_round_game_id_number",
                table: "round",
                columns: new[] { "game_id", "number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_round_game_id_number",
                table: "round");

            migrationBuilder.CreateIndex(
                name: "ix_round_game_id",
                table: "round",
                column: "game_id");
        }
    }
}
