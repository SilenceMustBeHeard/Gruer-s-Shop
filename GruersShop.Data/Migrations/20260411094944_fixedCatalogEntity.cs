using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GruersShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixedCatalogEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Catalogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Catalogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
