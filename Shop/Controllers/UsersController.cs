using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public ActionResult<GetUsersResponse> GetAll()
        {
            var users = _usersService.GetAllUsers();
            return Ok(new GetUsersResponse
            {
                Users = users.Select(u => u.ToDto())
            });
        }
    }
}
