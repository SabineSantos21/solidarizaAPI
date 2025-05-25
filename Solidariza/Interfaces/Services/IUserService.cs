using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByUserName(string username);
        Task<User?> GetUserByEmail(string email);
        Task<User> CreateUser(NewUser newUser);
    }
}
