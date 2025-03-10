using MongoDB.Driver;

namespace Ordering.Consumer.Repositories.Order;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Entities.Order> _orders;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(IMongoClient mongoClient, ILogger<OrderRepository> logger)
    {
        var database = mongoClient.GetDatabase("OrderDb");
        _orders = database.GetCollection<Entities.Order>("Orders");
        _logger = logger;
    }

    public async Task IsertOrUpdateAsync(Entities.Order order)
    {
        var foundOrder = await _orders.Find(o => o.CodigoPedido == order.CodigoPedido).FirstOrDefaultAsync();

        if (foundOrder != null)
        {
            order.Id = foundOrder.Id;
            
            _logger.LogInformation("Updating order {order}", order);
            await _orders.ReplaceOneAsync(x => x.CodigoPedido == order.CodigoPedido, order);
            return;
        }

        _logger.LogInformation("Inserting order {order}", order);
        await _orders.InsertOneAsync(order);
    }
}