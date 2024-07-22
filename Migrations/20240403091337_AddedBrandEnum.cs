using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneReclaim.Migrations
{
    /// <inheritdoc />
    public partial class AddedBrandEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Model",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Brand",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Products",
                type: "TEXT",
                nullable: true);
        }
    }
}
