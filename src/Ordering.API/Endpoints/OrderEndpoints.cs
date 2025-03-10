using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Repositories.Order;

namespace Ordering.API.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        string[] tags = ["Ordering"];

        app.MapGet("api/v1/orders", [Authorize] async ([FromServices] IOrderRepository repository) =>
            {
                var orders = await repository.GetOrdersAsync();
                return Results.Ok(orders);
            })
            .WithTags(tags);

        app.MapGet("api/v1/orders/{code}", [Authorize]
                async ([FromRoute] int code, [FromServices] IOrderRepository repository) =>
                {
                    var order = await repository.GetOrderByCode(code);

                    return order == null ? Results.NotFound() : Results.Ok(order);
                })
            .WithTags(tags);

        app.MapGet("api/v1/orders/customer/{customerCode}", [Authorize]
                async ([FromRoute] int customerCode, [FromServices] IOrderRepository repository) =>
                {
                    var orders = await repository.GetOrdersByCustomerCode(customerCode);
                    return Results.Ok(orders);
                })
            .WithTags(tags);

        app.MapGet("api/v1/orders/{code}/total", [Authorize]
                async ([FromRoute] int code, [FromServices] IOrderRepository repository) =>
                {
                    var total = await repository.GetTotalOrderValue(code);
                    return Results.Ok(total);
                })
            .WithTags(tags);

        return app;
    }
}