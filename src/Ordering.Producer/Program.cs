using MassTransit;
using Order.Consumer;
using Ordering.Producer.Endpoints;
using Ordering.Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.UsingRabbitMq((busCtx, rb) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>() ??
                             throw new InvalidOperationException("RabbitMq settings not found");

        rb.Host(rabbitMqConfig.Host, "/", h =>
        {
            h.Username(rabbitMqConfig.Username);
            h.Password(rabbitMqConfig.Password);
        });

        rb.ConfigureEndpoints(busCtx);
    });
});

var app = builder.Build();

app.MapOrderEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.Run();