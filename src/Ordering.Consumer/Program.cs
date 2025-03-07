using MassTransit;
using MongoDB.Driver;
using Ordering.Consumer;
using Ordering.Consumer.Consumers;
using Ordering.Consumer.Entities;
using Ordering.Consumer.Repositories.Order;
using Ordering.Shared.Events;

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
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<OrderCreatedConsumer>();

            busConfigurator.UsingRabbitMq((busCtx, rb) =>
            {
                var rabbitMqConfig = context.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>() ??
                                     throw new ArgumentNullException(nameof(context.Configuration));

                rb.Host(rabbitMqConfig.Host, "/", h =>
                {
                    h.Username(rabbitMqConfig.Username);
                    h.Password(rabbitMqConfig.Password);
                });
                
                rb.ConfigureEndpoints(busCtx);
            });
        });
    })
    .Build();

await host.RunAsync();