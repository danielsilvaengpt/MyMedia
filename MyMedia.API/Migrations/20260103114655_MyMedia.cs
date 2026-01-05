using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMedia.API.Migrations
{
    /// <inheritdoc />
    public partial class MyMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModoDisponibilizacaoId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ModosDisponibilizacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModosDisponibilizacao", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_ModoDisponibilizacaoId",
                table: "Produtos",
                column: "ModoDisponibilizacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                table: "Produtos",
                column: "ModoDisponibilizacaoId",
                principalTable: "ModosDisponibilizacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                table: "Produtos");

            migrationBuilder.DropTable(
                name: "ModosDisponibilizacao");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_ModoDisponibilizacaoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ModoDisponibilizacaoId",
                table: "Produtos");
        }
    }
}
