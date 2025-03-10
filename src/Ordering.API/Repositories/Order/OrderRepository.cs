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
        using var cursor = await _orders.FindAsync(x => true);
        return await cursor.ToListAsync();
    }

    public async Task<Entities.Order?> GetOrderByCode(int codigoPedido)
    {
        _logger.LogInformation($"Fetching order with code {codigoPedido}");
        using var cursor = await _orders.FindAsync(order => order.CodigoPedido == codigoPedido);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Entities.Order>> GetOrdersByCustomerCode(int codigoCliente)
    {
        _logger.LogInformation($"Fetching orders for customer with code {codigoCliente}");
        using var cursor = await _orders.FindAsync(order => order.CodigoCliente == codigoCliente);
        return await cursor.ToListAsync();
    }
}