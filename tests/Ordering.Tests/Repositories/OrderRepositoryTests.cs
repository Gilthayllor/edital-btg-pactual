using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Ordering.API.Entities;
using Ordering.API.Repositories.Order;

namespace Ordering.Tests.Repositories;

public class OrderRepositoryTests
{
    private readonly Mock<IMongoCollection<Order>> _mockOrderCollection;
    private readonly OrderRepository _orderRepository;

    public OrderRepositoryTests()
    {
        Mock<IMongoClient> mockMongoClient = new Mock<IMongoClient>();
        _mockOrderCollection = new Mock<IMongoCollection<Order>>();
        mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
            .Returns(Mock.Of<IMongoDatabase>(db =>
                db.GetCollection<Order>(It.IsAny<string>(), null) == _mockOrderCollection.Object));

        _orderRepository = new OrderRepository(mockMongoClient.Object, Mock.Of<ILogger<OrderRepository>>());
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { CodigoPedido = 1 },
            new Order { CodigoPedido = 2 }
        };
        var mockCursor = new Mock<IAsyncCursor<Order>>();
        mockCursor.Setup(x => x.Current).Returns(orders);
        mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockOrderCollection.Setup(x =>
                x.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _orderRepository.GetOrdersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetOrderByCode_ReturnsOrder()
    {
        //Arrange
        var order = new Order { CodigoPedido = 1 };
        var mockCursor = new Mock<IAsyncCursor<Order>>();
        mockCursor.Setup(x => x.Current).Returns(new List<Order> { order }.AsReadOnly());
        mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockOrderCollection.Setup(o => o.FindAsync(It.IsAny<FilterDefinition<Order>>(),
                It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(mockCursor.Object));

        //Act
        var result = await _orderRepository.GetOrderByCode(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result?.CodigoPedido);
    }

    [Fact]
    public async Task GetOrdersByCustomerCode_ReturnsOrders()
    {
        //Arrange
        var order = new Order { CodigoPedido = 1, CodigoCliente = 1 };
        var mockCursor = new Mock<IAsyncCursor<Order>>();
        mockCursor.Setup(x => x.Current).Returns(new List<Order> { order }.AsReadOnly());
        mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockOrderCollection.Setup(o => o.FindAsync(It.IsAny<FilterDefinition<Order>>(),
                It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(mockCursor.Object));

        //Act
        var result = await _orderRepository.GetOrdersByCustomerCode(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result?.First().CodigoCliente);
    }
}