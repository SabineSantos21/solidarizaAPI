using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface ILoginService
    {
        string GerarTokenJWT(string email);

        User? ValidarCredenciais(string email);
    }
}
