using MassTransit;
using MongoDB.Driver;
using Order.Consumer;
using Order.Consumer.Consumers;
using Order.Consumer.Repositories.Order;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IMongoClient>(s =>
        {
            var connectionString = context.Configuration.GetConnectionString("MongoDb") ??
                                   throw new ArgumentNullException(nameof(context.Configuration));
            
            return new MongoClient(connectionString);
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<OrderConsumer>();
            
            busConfigurator.UsingRabbitMq((busCtx, rb) =>
            {
                var rabbitMqConfig = context.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>() ??
                                     throw new ArgumentNullException(nameof(context.Configuration));

                rb.Host(rabbitMqConfig.Host, "/", h =>
                {
                    h.Username(rabbitMqConfig.Username);
                    h.Password(rabbitMqConfig.Password);
                });
                
                
                rb.ReceiveEndpoint("order-created-queue", ep =>
                {
                    ep.ConfigureConsumer<OrderConsumer>(busCtx);
                });
            });
        });
    })
    .Build();

await host.RunAsync();