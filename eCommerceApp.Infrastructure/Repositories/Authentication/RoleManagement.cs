using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using Microsoft.AspNetCore.Identity;

namespace eCommerceApp.Infrastructure.Repositories.Authentication
{
    public class RoleManagement(UserManager<AppUser> userManager) : IRoleManagement
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        public async Task<bool> AddUserToRole(AppUser user, string roleName) => 
            (await _userManager.AddToRoleAsync(user , roleName)).Succeeded;

        public async Task<string?> GetUserRole(string userEmail) 
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            return (await _userManager.GetRolesAsync(user!)).FirstOrDefault();
        }
    }
}
