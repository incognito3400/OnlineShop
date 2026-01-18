using Exam2.Backend.Entities;

namespace Shop.Interfaces
{
    public interface IUsersService
    {
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);
        User? GetUserByEmail(string email);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        bool ValidateCredentials(string email, string passwordHash);
    }
}
