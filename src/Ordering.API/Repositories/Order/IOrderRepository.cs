namespace Ordering.API.Repositories.Order;

public interface IOrderRepository
{
    Task<IEnumerable<Entities.Order>> GetOrdersAsync();
    Task<Entities.Order?> GetOrderByCode(int orderCode);
    Task<IEnumerable<Entities.Order>> GetOrdersByCustomerCode(int customerCode);
    Task<decimal> GetTotalOrderValue(int orderCode);
}