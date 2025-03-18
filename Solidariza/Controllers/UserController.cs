using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public UserController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            UserService userService = new UserService(_dbContext);

            User? user = await userService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateUser(NewUser newUser)
        {
            try
            {
                UserService usuarioService = new UserService(_dbContext);
                User user = await usuarioService.CreateUser(newUser);

                NewProfile newProfile = new NewProfile()
                {
                    UserId = user.UserId,
                    Name = user.Name
                };

                ProfileService profileService = new ProfileService(_dbContext);
                Profile profile = await profileService.CreateProfile(newProfile);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUsuario(int id, AtualizarUsuario atualizarUsuario)
        //{
        //    UserService usuarioService = new UserService(_dbContext);

        //    Usuario usuario = new Usuario();
        //    usuario.Email = atualizarUsuario.Email;
        //    usuario.Senha = atualizarUsuario.Senha;

        //    var existingUser = await _dbContext.TbUsuario.FindAsync(id);

        //    if (existingUser == null)
        //    {
        //        return NotFound();
        //    }

        //    await usuarioService.AtualizarUsuario(existingUser, usuario);

        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUsuario(int id)
        //{
        //    UserService usuarioService = new UserService(_dbContext);

        //    var usuario = await _dbContext.TbUsuario.FindAsync(id);

        //    if (usuario == null)
        //    {
        //        return NotFound();
        //    }

        //    await usuarioService.DeletarUsuario(usuario);

        //    return NoContent();
        //}
    }
}