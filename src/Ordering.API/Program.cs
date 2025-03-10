using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NSwag;
using NSwag.Generation.Processors.Security;
using Ordering.API;
using Ordering.API.Endpoints;
using Ordering.API.Middleware;
using Ordering.API.Repositories.Order;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(x =>
{
    x.Title = "Ordering API";

    x.AddSecurity("JWT", [], new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Scheme = "Bearer",
        Description = "Use the Get Token endpoint to get a JWT token and use it here: Bearer {your token}",
    });

    x.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<JwtSettings>(x => x.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb") ??
                           throw new InvalidCastException("MongoDb connection string not found");

    return new MongoClient(connectionString);
});

ConfigureAuthentication();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();

    app.MapGet("/", (context) =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.MapAuthEndpoints();
app.MapOrderEndpoints();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.Run();

void ConfigureAuthentication()
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:Key"] ??
                    throw new InvalidOperationException("JwtSettings:Key not found"))),
            };

            x.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = 401;
                    _ = context.Exception switch
                    {
                        SecurityTokenExpiredException => context.Response.Headers["Token-Expired"] = "true",
                        SecurityTokenInvalidAudienceException => context.Response.Headers["Token-Invalid-Audience"] =
                            "true",
                        SecurityTokenInvalidIssuerException =>
                            context.Response.Headers["Token-Invalid-Issuer"] = "true",
                        _ => context.Response.Headers["Token-Error"] = "true"
                    };

                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();
}