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
    
    public async Task InsertAsync(Entities.Order order)
    {
        _logger.LogInformation("Inserting order {order}", order);
        await _orders.InsertOneAsync(order);
    }
}