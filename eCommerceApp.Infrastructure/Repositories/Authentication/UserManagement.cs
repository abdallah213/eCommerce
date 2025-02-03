﻿using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eCommerceApp.Infrastructure.Repositories.Authentication
{
    public class UserManagement(IRoleManagement roleManagement , UserManager<AppUser> userManager , AppDbContext context) : IUserManagement
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IRoleManagement _roleManagement = roleManagement;
        private readonly AppDbContext _context = context;
        public async Task<bool> CreateUser(AppUser user)
        {
            var _user = await GetUserByEmail(user.Email!);
            if(_user != null)
            {
                return false;
            }

             return (await _userManager.CreateAsync(user , user.PasswordHash!)).Succeeded;
        }

        public async Task<IEnumerable<AppUser>?> GetAllUsers() => await _context.Users.ToListAsync();

        public async Task<AppUser?> GetUserByEmail(string email) => await _userManager.FindByEmailAsync(email);

        public async Task<AppUser> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user!;
        }

        public async Task<List<Claim>> GetUserClaims(string email)
        {
            var _user = await GetUserByEmail(email);
            string? roleName = await _roleManagement.GetUserRole(_user!.Email!);

            List<Claim> claims = [
                new Claim("FullName" , _user.FullName),
                new Claim(ClaimTypes.NameIdentifier , _user.Id), 
                new Claim(ClaimTypes.Email , _user!.Email!),
                new Claim(ClaimTypes.Role , roleName!)
                ];

            return claims;
        }

        public async Task<bool> LoginUser(AppUser user)
        {
            var _user = await GetUserByEmail(user.Email!);
            if(_user is null) return false;

            string? roleName = await _roleManagement.GetUserRole(_user.Email!);
            if(string.IsNullOrEmpty(roleName)) return false;

            return await _userManager.CheckPasswordAsync(_user, _user.PasswordHash!);
        }

        public async Task<int> RemoveUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(_=> _.Email == email);

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync();
        }
    }
}
