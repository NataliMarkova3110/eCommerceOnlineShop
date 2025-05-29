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
        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
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

        public async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await userManager.SetAuthenticationTokenAsync(user, "RefreshToken", "RefreshToken", refreshToken);
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshToken(ApplicationUser user, string refreshToken)
        {
            var storedToken = await userManager.GetAuthenticationTokenAsync(user, "RefreshToken", "RefreshToken");
            return storedToken == refreshToken;
        }
    }
}