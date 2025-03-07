using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class UsuarioService
    {
        private readonly ConnectionDB _dbContext;

        public UsuarioService(ConnectionDB dbContext) 
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

        public async Task<User> CreateUser(NewUser newUser)
        {
            User user = new User();
            user.Name = newUser.Name;
            user.Type = (UserType) newUser.Type;
            user.Email = newUser.Email;
            user.Password = newUser.Password;
            user.Phone = newUser.Phone;
            user.DocumentType = newUser.DocumentType;
            user.DocumentNumber = newUser.DocumentNumber;

            _dbContext.User.Add(user);
            await _dbContext.SaveChangesAsync();

            if ((UserType) newUser.Type == UserType.Organization)
            {
                OrganizationInfo organizationInfo = new OrganizationInfo();
                organizationInfo.UserId = user.UserId;
                organizationInfo.ContactName = newUser.ContactName;
                organizationInfo.ContactPhone = newUser.ContactPhone;

                _
            }

            return user;
        }

        //public async Task AtualizarUsuario(Usuario existingUsuario, Usuario usuario)
        //{
        //    existingUsuario.Nome = usuario.Nome;
        //    existingUsuario.Ativo = usuario.Ativo;
        //    existingUsuario.Data_modificacao = DateTime.Now;

        //    _dbContext.TbUsuario.Update(existingUsuario);
        //    await _dbContext.SaveChangesAsync();
        //}

        //public async Task DeletarUsuario(Usuario usuario)
        //{
        //    _dbContext.TbUsuario.Remove(usuario);
        //    await _dbContext.SaveChangesAsync();
        //}
    }
}
