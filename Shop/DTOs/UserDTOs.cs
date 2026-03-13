namespace Shop.DTOs
{
    // Responses
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class LoginResponse : UserDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class GetUsersResponse
    {
        public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();
    }

    // Requests
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Microsoft.AspNetCore.Http.IFormFile? Image { get; set; }
        // Optional: public string Role { get; set; }
    }
}
