using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class UserService
    {
        private readonly ConnectionDB _dbContext;

        public UserService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _dbContext.User.ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.UserId == id);
        }

        public async Task<User?> GetUserByUserName(string username)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.Name == username);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<User> CreateUser(NewUser newUser)
        {
  
            User user = new User()
            {
                Name = newUser.Name,
                Type = (UserType)newUser.Type,
                DocumentNumber = newUser.DocumentNumber,
                DocumentType = newUser.DocumentType != null ? (DocumentType)newUser.DocumentType : null,
                Phone = newUser.Phone,
                Email = newUser.Email,
                CreationDate = DateTime.UtcNow,
                IsActive = true,
                Password = newUser.Password,
            };

            _dbContext.User.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
         
        }

    }
}
