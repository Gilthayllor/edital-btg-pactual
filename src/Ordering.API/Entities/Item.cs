namespace Ordering.API.Entities;

public class Item
{
    public string Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
}