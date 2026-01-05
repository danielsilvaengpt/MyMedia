using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMedia.API.Migrations
{
    /// <inheritdoc />
    public partial class Bd122 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Produtos",
                newName: "ImagemContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "Imagem",
                table: "Produtos",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "ImagemContentType",
                table: "Produtos",
                newName: "ImageUrl");
        }
    }
}
