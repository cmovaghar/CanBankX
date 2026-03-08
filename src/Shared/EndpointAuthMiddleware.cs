using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public class EndpointAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        private readonly string _internalHeader = config["INTERNAL_HEADER"] ?? throw new InvalidOperationException("INTERNAL_HEADER is not set.");
        private readonly string _internalSecret = config["INTERNAL_SECRET"] ?? throw new InvalidOperationException("INTERNAL_SECRET is not set.");

        public async Task InvokeAsync(HttpContext ctx)
        {
            var endpoint = ctx.GetEndpoint();

            if (endpoint is null)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var meta = endpoint.Metadata;

            // Public - No auth
            if (meta.GetMetadata<PublicEndpointAttribute>() is not null)
            {
                await next(ctx);
                return;
            }

            // Login - JWT Token
            if (meta.GetMetadata<LoginEndpointAttribute>() is { } loginPolicy)
            {
                var result = await ctx.AuthenticateAsync();
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Auth failed: {result.Failure?.Message}");
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (loginPolicy.AllowedPurposes.Length > 0)
                {
                    var purpose = result.Principal.FindFirstValue("purpose");
                    if (!loginPolicy.AllowedPurposes.Contains(purpose))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await ctx.Response.WriteAsync("Insufficient token purpose.");
                        return;
                    }
                }

                ctx.User = result.Principal;
                await next(ctx);
                return;
            }

            // Internal - Header and internal secret
            if (meta.GetMetadata<InternalEndpointAttribute>() is not null)
            {
                var header = ctx.Request.Headers[_internalHeader].FirstOrDefault();
                if (string.IsNullOrEmpty(header) || !CryptographicEquals(header, _internalSecret))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
                await next(ctx);
                return;
            }

            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            await ctx.Response.WriteAsync("No endpoint policy defined.");
        }

        private static bool CryptographicEquals(string a, string b)
        {
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
        }
    }
}
