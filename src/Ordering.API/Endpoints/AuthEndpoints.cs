using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Ordering.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/get-token", (JwtSettings jwtSettings) =>
            {
                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Audience,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        SecurityAlgorithms.HmacSha256)
                );
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.WriteToken(jwtSecurityToken);
                
                return Results.Ok(token);
            })
            .WithTags("GetToken");

        return app;
    }
}