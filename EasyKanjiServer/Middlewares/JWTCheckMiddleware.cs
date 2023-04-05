using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;

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
            string header = context.Request.Headers.ContainsKey(HeaderNames.Authorization) ? context.Request.Headers[HeaderNames.Authorization].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(header) && header.StartsWith("Bearer"))
            {
                string jwt = header.Split(" ")[1];
                if (await _cache.GetStringAsync(jwt) == "invalid")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("JWT is invalid!");
                    return;
                }
            }
            await _next(context);
        }
    }
}
