using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafiStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddComparePriceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ComparePrice",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComparePrice",
                table: "Products");
        }
    }
}
