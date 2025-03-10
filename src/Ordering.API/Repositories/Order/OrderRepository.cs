using MongoDB.Driver;

namespace Ordering.API.Repositories.Order;

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

    public async Task<IEnumerable<Entities.Order>> GetOrdersAsync()
    {
        _logger.LogInformation("Fetching all orders");
        return await _orders.Find(x => true).ToListAsync();
    }

    public async Task<Entities.Order?> GetOrderByCode(int orderCode)
    {
        _logger.LogInformation("Fetching order by order code {orderCode}", orderCode);
        return await _orders.Find(x => x.CodigoPedido == orderCode).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Entities.Order>> GetOrdersByCustomerCode(int customerCode)
    {
        _logger.LogInformation("Fetching orders by Customer code {customerCode}", customerCode);
        return await _orders.Find(x => x.CodigoPedido == customerCode).ToListAsync();
    }

    public async Task<decimal> GetTotalOrderValue(int orderCode)
    {
        _logger.LogInformation("Fetching total order value by order code {orderCode}", orderCode);
        var order = await _orders.Find(x => x.CodigoPedido == orderCode).FirstOrDefaultAsync();

        return order?.Items.Sum(x => x.Preco) ?? 0;
    }
}