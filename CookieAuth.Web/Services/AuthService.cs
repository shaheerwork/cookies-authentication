using CookieAuth.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace CookieAuth.Web.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync();
        Task<UserProfile?> GetCurrentUserAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            // Configure the base address of the API
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001/");
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResult>() ?? 
                        new AuthResult { Succeeded = true };
                }
                
                return new AuthResult
                {
                    Succeeded = false,
                    Message = await response.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResult>() ?? 
                        new AuthResult { Succeeded = true };
                }
                
                return new AuthResult
                {
                    Succeeded = false,
                    Message = await response.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/auth/logout", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserProfile?> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/auth/user");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserProfile>();
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
