using MassTransit;
using Order.Consumer.Repositories.Order;

namespace Order.Consumer.Consumers;

public class OrderConsumer(IOrderRepository orderRepository, ILogger<OrderConsumer> logger) : IConsumer<Entities.Order>
{
    public async Task Consume(ConsumeContext<Entities.Order> context)
    {
        try
        {
            await orderRepository.InsertAsync(context.Message);
            logger.LogInformation("Order {codigoPedido} Inserted", context.Message.CodigoPedido);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
