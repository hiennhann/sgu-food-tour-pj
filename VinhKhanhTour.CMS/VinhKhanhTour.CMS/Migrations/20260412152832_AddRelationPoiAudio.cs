using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationPoiAudio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Audios_PoiId",
                table: "Audios",
                column: "PoiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audios_Pois_PoiId",
                table: "Audios",
                column: "PoiId",
                principalTable: "Pois",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audios_Pois_PoiId",
                table: "Audios");

            migrationBuilder.DropIndex(
                name: "IX_Audios_PoiId",
                table: "Audios");
        }
    }
}
