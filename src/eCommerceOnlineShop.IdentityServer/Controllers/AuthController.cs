using eCommerceOnlineShop.IdentityServer.Models;
using eCommerceOnlineShop.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceOnlineShop.IdentityServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleService roleService,
        TokenService tokenService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await roleService.AssignRoleToUserAsync(user.Id, "Customer");
                return Ok(new { message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var token = await tokenService.GenerateJwtTokenAsync(user);
                var refreshToken = await tokenService.GenerateRefreshTokenAsync(user);

                return Ok(new
                {
                    token,
                    refreshToken
                });
            }

            return Unauthorized(new { message = "Invalid email or password" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            if (!await tokenService.ValidateRefreshTokenAsync(user, model.RefreshToken))
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            var token = await tokenService.GenerateJwtTokenAsync(user);
            var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(user);

            return Ok(new
            {
                token,
                refreshToken = newRefreshToken
            });
        }
    }
}
