namespace MyMedia.Shared.DTOs
{
    public class EncomendaDto
    {

        // Dados do Checkout
        public string MetodoPagamento { get; set; } = "MBWay"; // Default
        public decimal ValorTotal { get; set; }
        public string MoradaEntrega { get; set; }

        // A lista de coisas que ele vai comprar
        public List<ItemCarrinhoDto> Itens { get; set; } = new List<ItemCarrinhoDto>();
    }

    public class ItemCarrinhoDto
    {
        public int ProdutoId { get; set; }
        public string Titulo { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}