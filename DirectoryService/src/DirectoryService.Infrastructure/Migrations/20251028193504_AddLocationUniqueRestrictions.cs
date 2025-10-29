using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationUniqueRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_address",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                table: "locations");
        }
    }
}
