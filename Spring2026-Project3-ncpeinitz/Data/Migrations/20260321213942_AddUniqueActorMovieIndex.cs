using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spring2026_Project3_ncpeinitz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueActorMovieIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActorMovie_ActorId",
                table: "ActorMovie");

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovie_ActorId_MovieId",
                table: "ActorMovie",
                columns: new[] { "ActorId", "MovieId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActorMovie_ActorId_MovieId",
                table: "ActorMovie");

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovie_ActorId",
                table: "ActorMovie",
                column: "ActorId");
        }
    }
}
