using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IImageService _imageService;

        public AuthController(IUsersService usersService, IImageService imageService)
        {
            _usersService = usersService;
            _imageService = imageService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromForm] RegisterRequest request)
        {
            var existingUser = _usersService.GetUserByEmail(request.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User with this email already exists" });

            string? imageUrl = null;
            if (request.Image != null)
            {
                imageUrl = await _imageService.UploadImageAsync(request.Image);
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Role = "User",
                ImageUrl = imageUrl
            };

            _usersService.AddUser(user);

            return Created($"/api/users/{user.Id}", user.ToDto());
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            var user = _usersService.GetUserByEmail(request.Email);
            if (user == null || user.PasswordHash != request.Password) 
                return Unauthorized();
                // test 
            var token = $"fake-jwt-token-{user.Id}"; 
            return Ok(user.ToLoginResponse(token));
        }
    }
}
