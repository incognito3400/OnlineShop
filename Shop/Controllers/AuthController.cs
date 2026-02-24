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

        public AuthController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("register")]
        public ActionResult<UserDto> Register([FromBody] RegisterRequest request)
        {
            var existingUser = _usersService.GetUserByEmail(request.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User with this email already exists" });

            var user = new User
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Role = "User"
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
