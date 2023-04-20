using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EasyKanjiServer.Middlewares
{
    public class JWTCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public JWTCheckMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string name = context.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            string header = context.Request.Headers.ContainsKey(HeaderNames.Authorization) ? context.Request.Headers[HeaderNames.Authorization].ToString() : string.Empty;
            string jwt = !string.IsNullOrEmpty(header) && header.StartsWith("Bearer") ? header.Split(" ")[1] : string.Empty;

            if (await _cache.GetStringAsync(name) == "invalid" && !string.IsNullOrEmpty(jwt))
            {
                await _cache.SetStringAsync(jwt, "invalid", new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = new JwtSecurityTokenHandler().ReadJwtToken(jwt).ValidTo
                });
                await _cache.RemoveAsync(name);
            }

            if (await _cache.GetStringAsync(jwt) == "invalid")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { errors = "JWT is invalid." });

                return;
            }

            await _next(context);
        }
    }
}
