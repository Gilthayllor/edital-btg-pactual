using MassTransit;
using MassTransit.RabbitMqTransport.Topology;
using MassTransit.Transports.Fabric;
using Order.Consumer;
using Order.Consumer.Events;
using Order.Producer.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingRabbitMq((busCtx, rb) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>() ??
                             throw new ArgumentNullException(nameof(builder.Configuration));

        rb.Host(rabbitMqConfig.Host, "/", h =>
        {
            h.Username(rabbitMqConfig.Username);
            h.Password(rabbitMqConfig.Password);
        });
        
        rb.Publish<OrderCreated>(x =>
        {
            x.ExchangeType = "direct";
        });
    });
});

var app = builder.Build();

app.MapEndpoints();

app.UseOpenApi();
app.UseSwaggerUi();

app.Run();