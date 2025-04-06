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

                if (user.Type == UserType.Organization)
                {
                    ValidateOrganizationService validateOrganizationService = new ValidateOrganizationService(_dbContext);
                    ConsultCNPJResponse organizationValid = await validateOrganizationService.ConsultCNPJ(user.DocumentNumber);

                    NewOrganizationInfoCNPJValid newOrganizationInfo = new NewOrganizationInfoCNPJValid()
                    {
                        UserId = user.UserId,
                        IsOrganizationApproved = organizationValid.IsValid,
                        DisapprovalReason = organizationValid.DisapprovalReason,
                    };

                    OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext); 
                    OrganizationInfo organizationInfo = await organizationInfoService.CreateOrganizationInfoCPNJValid(newOrganizationInfo);
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