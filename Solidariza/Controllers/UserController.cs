using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using Solidariza.Common;

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

                if (string.IsNullOrEmpty(newUser.Email))
                {
                    return BadRequest("O e-mail é obrigatório.");
                }

                User? userVerify = await usuarioService.GetUserByEmail(newUser.Email);
                
                if(userVerify != null)
                {
                    return BadRequest("Este e-mail não está mais disponível para cadastro. Por favor, utilize outro e-mail ou, se já possui um cadastro, recupere o acesso ao anterior.");
                }

                if (string.IsNullOrEmpty(newUser.Password))
                {
                    return BadRequest("A senha é obrigatória.");
                }

                newUser.Password = PasswordHash.HashPassword(newUser.Password);

                User user = await usuarioService.CreateUser(newUser);

                NewProfile newProfile = new NewProfile()
                {
                    UserId = user.UserId,
                    Name = user.Name
                };

                ProfileService profileService = new ProfileService(_dbContext);
                await profileService.CreateProfile(newProfile);

                if (user.Type == UserType.Organization)
                {
                    if (user.DocumentNumber == null) return BadRequest("Número do documento é obrigatório");

                    ValidateOrganizationService validateOrganizationService = new ValidateOrganizationService();
                    ConsultCNPJResponse organizationValid = await validateOrganizationService.ConsultCNPJ(user.DocumentNumber);

                    NewOrganizationInfoCNPJValid newOrganizationInfo = new NewOrganizationInfoCNPJValid()
                    {
                        UserId = user.UserId,
                        IsOrganizationApproved = organizationValid.IsValid,
                        DisapprovalReason = organizationValid.DisapprovalReason,
                    };

                    OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext); 
                    await organizationInfoService.CreateOrganizationInfoCPNJValid(newOrganizationInfo);
                }

                return Ok(user);
            }
            catch (Exception ex)
            {   
                return Problem(ex.Message);
            }
            
        }
    }
}