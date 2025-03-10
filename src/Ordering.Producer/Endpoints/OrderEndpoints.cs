using MassTransit;
using Ordering.Shared.Events;

namespace Ordering.Producer.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/v1/create-order", async (OrderRequest order, IBus publisher) =>
            {
                await publisher.Publish(new OrderCreated
                {
                    CodigoPedido = order.CodigoPedido,
                    CodigoCliente = order.CodigoCliente,
                    Items = order.Items.Select(i => new OrderCreated.Item
                        {
                            Produto = i.Produto,
                            Quantidade = i.Quantidade,
                            Preco = i.Preco
                        })
                        .ToList()
                });
            })
            .WithTags("Ordering");
    }

    public record OrderRequest
    {
        public int CodigoPedido { get; init; }
        public int CodigoCliente { get; init; }
        public IEnumerable<Item> Items { get; init; } = [];

        public record Item
        {
            public string Produto { get; init; } = null!;
            public int Quantidade { get; set; }
            public decimal Preco { get; set; }
        }
    }
}