namespace Ordering.Shared.Events;

public record OrderCreated
{
    public int CodigoPedido { get; init; }
    public int CodigoCliente { get; init; }
    public List<Item> Items { get; init; } = [];

    public record Item
    {
        public string Produto { get; init; } = null!;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}