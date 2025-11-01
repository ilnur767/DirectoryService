using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionUniqueRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_address",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_positions_name",
                table: "positions",
                column: "name",
                unique: true,
                filter: "\"is_active\" IS TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_locations_address",
                table: "locations",
                column: "address",
                unique: true,
                filter: "\"is_active\" IS TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                table: "locations",
                column: "name",
                unique: true,
                filter: "\"is_active\" IS TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_positions_name",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_locations_address",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_locations_address",
                table: "locations",
                column: "address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                table: "locations",
                column: "name",
                unique: true);
        }
    }
}
