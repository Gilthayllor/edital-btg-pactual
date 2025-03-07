using System.Text.Json;
using MassTransit;
using Ordering.Consumer.Entities;
using Ordering.Consumer.Repositories.Order;
using Ordering.Shared.Events;

namespace Ordering.Consumer.Consumers;

public class OrderCreatedConsumer(IOrderRepository orderRepository, ILogger<OrderCreatedConsumer> logger)
    : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        try
        {
            var newOrder = new Order()
            { 
                CodigoPedido = context.Message.CodigoPedido,
                CodigoCliente = context.Message.CodigoCliente,
                Itens = context.Message.Itens.Select(x => new Item
                {
                    Preco = x.Preco,
                    Quantidade = x.Quantidade,
                    Produto = x.Produto
                }).ToList()
            };

            await orderRepository.InsertAsync(newOrder);
            logger.LogInformation("Order {codigoPedido} Inserted", context.Message.CodigoPedido);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}