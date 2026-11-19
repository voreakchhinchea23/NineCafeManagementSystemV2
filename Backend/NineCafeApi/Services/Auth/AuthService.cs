using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Auth;

namespace NineCafeApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // 1. Find user by username with UserRole -> Role -> Permissions
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r!.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.Trim().ToLower());

            if (user == null || !user.IsActive)
            {
                return null;
            }

            // 2. Verify password with BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            // 3. Extract the single role & permissions
            var role = user.UserRoles.FirstOrDefault()?.Role;
            var roleName = role?.Name ?? "NoRole";

            var permissions = role?.RolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.Name)
                .ToList() ?? new List<string>();

            // 4. Build JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };

            // Add each permission as a "permission" claim
            foreach (var perm in permissions)
            {
                claims.Add(new Claim("permission", perm));
            }

            // 5. Generate signed JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(8); // Valid for 8-hour shift

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new AuthResponseDto
            {
                Token = tokenString,
                Expiration = expires,
                UserId = user.PublicId,
                Username = user.Username,
                FullName = user.FullName,
                Role = roleName,
                Permissions = permissions
            };
        }
    }
}