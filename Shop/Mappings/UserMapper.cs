using Exam2.Backend.Entities;
using Shop.DTOs;

namespace Shop.Mappings
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }

        public static LoginResponse ToLoginResponse(this User user, string token)
        {
            return new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }
    }
}
