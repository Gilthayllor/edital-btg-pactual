namespace Order.Consumer.Repositories.Order;

public interface IOrderRepository
{
    Task InsertAsync(Entities.Order order);
}