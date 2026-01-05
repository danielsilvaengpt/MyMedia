using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMedia.API.Migrations
{
    /// <inheritdoc />
    public partial class CarrinhoEncomenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetodoPagamento",
                table: "Encomendas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MoradaEntrega",
                table: "Encomendas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DetalhesEncomenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncomendaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalhesEncomenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalhesEncomenda_Encomendas_EncomendaId",
                        column: x => x.EncomendaId,
                        principalTable: "Encomendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalhesEncomenda_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesEncomenda_EncomendaId",
                table: "DetalhesEncomenda",
                column: "EncomendaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesEncomenda_ProdutoId",
                table: "DetalhesEncomenda",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalhesEncomenda");

            migrationBuilder.DropColumn(
                name: "MetodoPagamento",
                table: "Encomendas");

            migrationBuilder.DropColumn(
                name: "MoradaEntrega",
                table: "Encomendas");
        }
    }
}
