using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public UsuarioController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            UsuarioService usuarioService = new UsuarioService(_dbContext);

            IEnumerable<Usuario> usuarios = await usuarioService.GetUsuarios();

            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            UsuarioService usuarioService = new UsuarioService(_dbContext);

            Usuario? usuario = await usuarioService.GetUsuarioById(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult> CriarUsuario(NovoUsuario novoUsuario)
        {
            UsuarioService usuarioService = new UsuarioService(_dbContext);

            Usuario usuario = new Usuario();
            usuario.Nome = novoUsuario.Nome;
            usuario.Email = novoUsuario.Email;
            usuario.Senha = novoUsuario.Senha;
            usuario.Data_criacao = DateTime.Now;
            usuario.Data_modificacao = DateTime.Now;
            usuario.Ativo = true;

            await usuarioService.CriarUsuario(usuario);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, AtualizarUsuario atualizarUsuario)
        {
            UsuarioService usuarioService = new UsuarioService(_dbContext);

            Usuario usuario = new Usuario();
            usuario.Email = atualizarUsuario.Email;
            usuario.Senha = atualizarUsuario.Senha;

            var existingUser = await _dbContext.TbUsuario.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            await usuarioService.AtualizarUsuario(existingUser, usuario);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            UsuarioService usuarioService = new UsuarioService(_dbContext);

            var usuario = await _dbContext.TbUsuario.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            await usuarioService.DeletarUsuario(usuario);

            return NoContent();
        }
    }
}