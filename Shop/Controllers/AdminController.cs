using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public AdminController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = _usersService.GetAllUsers();
            return Ok(users.Select(u => u.ToDto()));
        }

        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _usersService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (user.Role == "Admin")
            {
                return BadRequest(new { message = "Cannot delete an admin account" });
            }

            _usersService.DeleteUser(id);
            return Ok(new { message = "User deleted successfully" });
        }
    }
}
