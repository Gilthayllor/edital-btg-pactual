namespace Ordering.Shared.Events;

public record OrderCreated
{
    public int CodigoPedido { get; init; }
    public int CodigoCliente { get; init; }
    public List<Item> Itens { get; init; }

    public record Item
    {
        public string Produto { get; init; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}