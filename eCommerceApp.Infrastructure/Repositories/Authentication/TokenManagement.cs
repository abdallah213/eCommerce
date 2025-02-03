using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eCommerceApp.Infrastructure.Repositories.Authentication
{
    public class TokenManagement(AppDbContext context , IConfiguration config) : ITokenManagement
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _config = config;
        public Task<int> AddRefreshToken(string userId, string refreshToken)
        {
            _context.RefreshToken
                .Add(new RefreshToken { UserId = userId, Token = refreshToken });

            return _context.SaveChangesAsync();
        }

        public string GenerateToken(List<Claim> claims)
        {
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var cred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(2);
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken()
        {
            const int byteSize = 64;
            byte[] randomBytes = new byte[byteSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public List<Claim> GetUserClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            if (jwtToken is not null)
                return jwtToken.Claims.ToList();
            else
                return [];
        }

        public async Task<string> GetUserIdByRefreshToken(string refreshToken)
            => (await _context.RefreshToken.FirstOrDefaultAsync(_ => _.Token == refreshToken))!.UserId;

        public async Task<int> UpdateRefreshToken(string userId, string refreshToken)
        {
            var user = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (user == null) return -1;

            user.Token = refreshToken;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidDateRefreshToken(string refreshToken)
        {
            var user = await _context.RefreshToken
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            return user is not null;
        }
    }
}
