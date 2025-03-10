namespace Ordering.Consumer.Repositories.Order;

public interface IOrderRepository
{
    Task IsertOrUpdateAsync(Entities.Order order);
}