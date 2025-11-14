using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLtreeExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS ltree;");

            migrationBuilder.Sql(@"
                ALTER TABLE departments
                ALTER COLUMN path TYPE ltree
                USING path::ltree;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE departments
                ALTER COLUMN path TYPE varchar(1000);
            ");

            migrationBuilder.Sql("DROP EXTENSION IF EXISTS ltree;");
        }
    }
}
