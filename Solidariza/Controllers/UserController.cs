using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using Solidariza.Common;
using Solidariza.Interfaces.Services;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProfileService _profileService;
        private readonly IValidateOrganizationService _validateOrganizationService;
        private readonly IOrganizationInfoService _organizationInfoService;

        public UserController(
            IUserService userService,
            IProfileService profileService,
            IValidateOrganizationService validateOrganizationService,
            IOrganizationInfoService organizationInfoService)
        {
            _userService = userService;
            _profileService = profileService;
            _validateOrganizationService = validateOrganizationService;
            _organizationInfoService = organizationInfoService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            User? user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(NewUser newUser)
        {
            try
            {
                if (string.IsNullOrEmpty(newUser.Email))
                {
                    return BadRequest("O e-mail é obrigatório.");
                }

                User? userVerify = await _userService.GetUserByEmail(newUser.Email);

                if (userVerify != null)
                {
                    return BadRequest("Este e-mail não está mais disponível para cadastro. Por favor, utilize outro e-mail ou, se já possui um cadastro, recupere o acesso ao anterior.");
                }

                if (string.IsNullOrEmpty(newUser.Password))
                {
                    return BadRequest("A senha é obrigatória.");
                }

                newUser.Password = PasswordHash.HashPassword(newUser.Password);

                User user = await _userService.CreateUser(newUser);

                NewProfile newProfile = new NewProfile
                {
                    UserId = user.UserId,
                    Name = user.Name
                };

                await _profileService.CreateProfile(newProfile);

                if (user.Type == UserType.Organization)
                {
                    if (string.IsNullOrEmpty(user.DocumentNumber))
                    {
                        return BadRequest("Número do documento é obrigatório para organizações.");
                    }

                    ConsultCnpjResponse organizationValid = await _validateOrganizationService.ConsultCNPJ(user.DocumentNumber);

                    NewOrganizationInfoCnpjValid newOrganizationInfo = new NewOrganizationInfoCnpjValid
                    {
                        UserId = user.UserId,
                        IsOrganizationApproved = organizationValid.IsValid,
                        DisapprovalReason = organizationValid.DisapprovalReason
                    };

                    await _organizationInfoService.CreateOrganizationInfoCPNJValid(newOrganizationInfo);
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