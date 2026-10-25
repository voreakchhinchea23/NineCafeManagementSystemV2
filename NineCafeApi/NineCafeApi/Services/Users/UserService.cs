using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Users;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL USERS
        public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.Name : "No Role",
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();
        }

        // GET USER BY ID
        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RoleId = user.RoleId,
                RoleName = user.Role != null ? user.Role.Name : "No Role",
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        // CREATE USER
        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            // 1. Verify Role exists
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {dto.RoleId} does not exist.");
            }

            var trimmedUsername = dto.Username.Trim();
            var trimmedEmail = dto.Email.Trim().ToLower();

            // 2. Check Username uniqueness
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == trimmedUsername.ToLower());
            if (usernameExists)
            {
                throw new InvalidOperationException($"Username '{dto.Username}' is already taken.");
            }

            // 3. Check Email uniqueness
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == trimmedEmail);
            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{dto.Email}' is already registered.");
            }
            
            // 4.Check PhoneNumber uniqueness
            var trimmedPhone = dto.PhoneNumber.Trim();

            var phoneExists = await _context.Users
                .AnyAsync(u => u.PhoneNumber == trimmedPhone);
            if (phoneExists)
            {
                throw new InvalidOperationException($"Phone number '{dto.PhoneNumber}' is already in use.");
            }

            // 5. Hash password with BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);


            var user = new User
            {
                Username = trimmedUsername,
                FullName = dto.FullName.Trim(),
                Email = trimmedEmail,
                PasswordHash = passwordHash,
                PhoneNumber = trimmedPhone, 
                RoleId = dto.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RoleId = user.RoleId,
                RoleName = role.Name,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        // UPDATE USER (Profile info, Role, Status)
        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            var trimmedEmail = dto.Email.Trim().ToLower();
            var trimmedPhone = dto.PhoneNumber.Trim();
            var trimmedUsername = dto.Username.Trim();

            // 1. Verify Role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists)
            {
                throw new KeyNotFoundException($"Role with ID {dto.RoleId} does not exist.");

            }
            // 2. Check Email uniqueness on other users
            var emailExists = await _context.Users
                .AnyAsync(u => u.Id != id && u.Email.ToLower() == trimmedEmail);
            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{dto.Email}' is already registered to another user.");
            }

            // 3. Check if any other user has this phone number
            var phoneExists = await _context.Users
                .AnyAsync(u => u.Id != id && u.PhoneNumber == trimmedPhone);
            if (phoneExists)
            {
                throw new InvalidOperationException($"Phone number '{dto.PhoneNumber}' is already in use by another user.");
            }

            //4.CHECK USERNAME UNIQUENESS(Excluding current user)
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Id != id && u.Username.ToLower() == trimmedUsername.ToLower());
            if (usernameExists)
            {
                throw new InvalidOperationException($"Username '{dto.Username}' is already taken by another user.");
            }

            user.Username = trimmedUsername;
            user.FullName = dto.FullName.Trim();
            user.Email = trimmedEmail;
            user.PhoneNumber = trimmedPhone;
            user.IsActive = dto.IsActive;
            user.RoleId = dto.RoleId;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE USER (With Last Admin Protection)
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return false;

            // Safety rule: Prevent deleting the last Admin account
            if (user.Role?.Name == "Admin")
            {
                var adminCount = await _context.Users.CountAsync(u => u.Role!.Name == "Admin");
                if (adminCount <= 1)
                {
                    throw new InvalidOperationException("Action denied: Cannot delete the last remaining Administrator account.");
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // CHANGE PASSWORD
        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // Verify current password
            bool isCorrect = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!isCorrect)
            {
                throw new InvalidOperationException("Current password does not match.");
            }

            // Hash and set new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}