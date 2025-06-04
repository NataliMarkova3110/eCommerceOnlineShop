using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eCommerceOnlineShop.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace eCommerceOnlineShop.IdentityServer.Services
{
    public class TokenService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured. Please set the 'Jwt:Key' configuration value.")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await userManager.SetAuthenticationTokenAsync(user, "RefreshToken", "RefreshToken", refreshToken);
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            var storedToken = await userManager.GetAuthenticationTokenAsync(user, "RefreshToken", "RefreshToken");
            return storedToken == refreshToken;
        }
    }
}
