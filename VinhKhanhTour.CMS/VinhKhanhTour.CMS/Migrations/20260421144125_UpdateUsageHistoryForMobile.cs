using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsageHistoryForMobile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "UsageHistories",
                newName: "IpAddress");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "UsageHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "UsageHistories");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "UsageHistories",
                newName: "UserEmail");
        }
    }
}
