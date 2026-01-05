using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMedia.API.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarCamposUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "NIF",
                table: "AspNetUsers",
                newName: "Nif");

            migrationBuilder.AlterColumn<int>(
                name: "ModoDisponibilizacaoId",
                table: "Produtos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NifEmpresa",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeEmpresa",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoUtilizador",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                table: "Produtos",
                column: "ModoDisponibilizacaoId",
                principalTable: "ModosDisponibilizacao",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NifEmpresa",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NomeEmpresa",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TipoUtilizador",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Nif",
                table: "AspNetUsers",
                newName: "NIF");

            migrationBuilder.AlterColumn<int>(
                name: "ModoDisponibilizacaoId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                table: "Produtos",
                column: "ModoDisponibilizacaoId",
                principalTable: "ModosDisponibilizacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
