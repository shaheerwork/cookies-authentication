using CookieAuth.Shared.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace CookieAuth.API.Services
{
    public interface IUserService
    {
        Task<AuthResult> RegisterUserAsync(RegisterRequest request);
        Task<AuthResult> ValidateUserAsync(LoginRequest request);
        Task<UserProfile?> GetUserByIdAsync(string id);
    }

    public class UserService : IUserService
    {
        // In a real application, this would be a database
        private static readonly ConcurrentDictionary<string, UserRecord> _users = new();

        public Task<AuthResult> RegisterUserAsync(RegisterRequest request)
        {
            // Generate a unique ID for the user
            var userId = Guid.NewGuid().ToString();
            
            // Check if email is already registered
            if (_users.Values.Any(u => u.Email == request.Email))
            {
                return Task.FromResult(new AuthResult
                {
                    Succeeded = false,
                    Message = "Email is already registered"
                });
            }

            // Hash the password (in production, use a proper password hasher)
            var passwordHash = HashPassword(request.Password);
            
            // Create and store the user
            var newUser = new UserRecord
            {
                Id = userId,
                Email = request.Email,
                PasswordHash = passwordHash,
                FullName = request.FullName
            };
            
            _users[userId] = newUser;
            
            return Task.FromResult(new AuthResult
            {
                Succeeded = true,
                User = new UserProfile
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    FullName = newUser.FullName
                }
            });
        }

        public Task<AuthResult> ValidateUserAsync(LoginRequest request)
        {
            // Find user by email
            var user = _users.Values.FirstOrDefault(u => u.Email == request.Email);
            
            if (user == null)
            {
                return Task.FromResult(new AuthResult
                {
                    Succeeded = false,
                    Message = "Invalid email or password"
                });
            }
            
            // Verify password
            var passwordHash = HashPassword(request.Password);
            if (user.PasswordHash != passwordHash)
            {
                return Task.FromResult(new AuthResult
                {
                    Succeeded = false,
                    Message = "Invalid email or password"
                });
            }
            
            return Task.FromResult(new AuthResult
            {
                Succeeded = true,
                User = new UserProfile
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName
                }
            });
        }

        public Task<UserProfile?> GetUserByIdAsync(string id)
        {
            if (_users.TryGetValue(id, out var user))
            {
                return Task.FromResult<UserProfile?>(new UserProfile
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName
                });
            }
            
            return Task.FromResult<UserProfile?>(null);
        }

        // In a real application, use a secure password hashing algorithm like BCrypt
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    // Internal user record with password hash
    internal class UserRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
