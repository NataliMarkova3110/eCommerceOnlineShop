using Microsoft.AspNetCore.Identity;
using eCommerceOnlineShop.IdentityServer.Models;

namespace eCommerceOnlineShop.IdentityServer.Services
{
    public class RoleService(
        UserManager<ApplicationUser> userManager)
    {

        public async Task AssignRoleToUser(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task<bool> HasPermission(string userId, string permission)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Manager"))
            {
                return true;
            }
            else if (roles.Contains("Customer"))
            {
                return permission == "Read";
            }

            return false;
        }
    }
}