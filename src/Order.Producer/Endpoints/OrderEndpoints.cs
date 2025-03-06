using MassTransit;
using Order.Consumer.Events;

namespace Order.Producer.Endpoints;

public static class OrderEndpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/create-order", async (OrderDTO order, IBus publisher) =>
        {
            await publisher.Publish(new OrderCreated
            {
                CodigoPedido = order.CodigoPedido,
                CodigoCliente = order.CodigoCliente,
                Itens = order.Itens.Select(i => new OrderCreated.Item
                {
                    Produto = i.Produto,
                    Quantidade = i.Quantidade,
                    Preco = i.Preco
                })
                .ToList()   
            });
        })
        .WithTags("order", "create");
    }

    public record OrderDTO
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
}