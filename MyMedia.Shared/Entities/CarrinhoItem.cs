namespace MyMedia.Shared.Entities
{
    public class CarrinhoItem
    {
        public Produto Produto { get; set; } = new();
        public int Quantidade { get; set; } = 1;

        // Calcula o total deste item (Preço x Quantidade)
        // ATENÇÃO: Se a tua propriedade no Produto for 'Price' ou 'PrecoVenda', altera aqui!
        public decimal Subtotal => Produto.PrecoFinal * Quantidade;
    }
}