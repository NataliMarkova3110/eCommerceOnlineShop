using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace eCommerceOnlineShop.Cart.Middleware
{
    public class TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.ReadToken(token) is JwtSecurityToken jsonToken)
                    {
                        var tokenDetails = new
                        {
                            UserId = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value,
                            Email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                            Roles = jsonToken.Claims.Where(c => c.Type == "role").Select(c => c.Value),
                            IssuedAt = jsonToken.IssuedAt,
                            ExpiresAt = jsonToken.ValidTo
                        };

                        logger.LogInformation(
                            "Token Details - UserId: {UserId}, Email: {Email}, Roles: {Roles}, IssuedAt: {IssuedAt}, ExpiresAt: {ExpiresAt}",
                            tokenDetails.UserId,
                            tokenDetails.Email,
                            string.Join(", ", tokenDetails.Roles),
                            tokenDetails.IssuedAt,
                            tokenDetails.ExpiresAt);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing token in middleware");
                }
            }

            await next(context);
        }
    }
}