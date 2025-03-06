using MongoDB.Driver;

namespace Order.Consumer.Repositories.Order;

public class OrderRepository : IOrderRepository
{
    private IMongoCollection<Entities.Order> _orders;
    
    public OrderRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("OrderDb");
        _orders = database.GetCollection<Entities.Order>("Orders");
    }
    
    public async Task InsertAsync(Entities.Order order)
    {
        await _orders.InsertOneAsync(order);
    }
}