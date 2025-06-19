using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System;

namespace CourseraWebApi.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizationMiddleware> _logger;
        // In a real application, these would come from configuration (e.g., appsettings.json)
        private readonly string _jwtSecret = "thisismyverysecretkeyforauthenticationpurposes1234567890"; // NEVER hardcode in production
        private readonly string _jwtAudience = "your-audience"; // e.g., "api.yourapp.com"
        private readonly string _jwtIssuer = "your-issuer"; // e.g., "yourapp.com"

        public AuthorizationMiddleware(RequestDelegate next, ILogger<AuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for specific paths, e.g., login or public endpoints
            if (context.Request.Path.StartsWithSegments("/api/auth/login") ||
                context.Request.Path.StartsWithSegments("/api/public"))
            {
                await _next(context);
                return;
            }

            string authHeader = context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Missing or invalid Authorization header.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: No token provided or invalid format.");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var identity = new ClaimsIdentity(jwtToken.Claims, "Token");

                // Attach validated principal to HttpContext.User
                context.User = new ClaimsPrincipal(identity);

                _logger.LogInformation($"Token validated successfully for user: {context.User.Identity?.Name ?? "Unknown"}");

                await _next(context); // Continue to the next middleware in the pipeline
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogError($"Token expired: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Token has expired.");
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid token.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred during token validation: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An error occurred during authentication.");
            }
        }
    }

    // Extension method to easily add the middleware in Startup.cs
    public static class AuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}